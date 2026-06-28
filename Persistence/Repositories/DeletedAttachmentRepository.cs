using Domain.Models.Documents;
using Domain.Models.Documents.Keys;
using Domain.Models.Types;
using Domain.Stores.MongoDB;
using Infrastructure.Database;
using MongoDB.Driver;

namespace Persistence.Repositories
{
    public class DeletedAttachmentRepository : IDeletedAttachmentStore
    {
        private readonly IMongoCollection<DeletedAttachment> _collection;

        public DeletedAttachmentRepository(UpdatesContext context)
        {
            _collection = context.DeletedAttachments;
        }

        public async Task<DeletedAttachment?> GetOneEldestAsync(CancellationToken cancellationToken)
        {
            return await _collection.Find(FilterDefinition<DeletedAttachment>.Empty)
                .SortBy(x => x.DeletedAt).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Guid>> GetDeletedByMessageIdAsync(Guid messageId, EnChatType chatType, Guid chatId, CancellationToken cancellationToken)
        {
            return await _collection
                .Find(x => x.Id.ChatType == chatType 
                    && x.Id.ChatId == chatId 
                    && x.Id.MessageId == messageId)
                .Project(m=>m.Id.MediaId).ToListAsync(cancellationToken);
        }

        public async Task<bool> CreateAsync(DeletedAttachment deletedAttachment, CancellationToken cancellationToken)
        {
            try
            {
                await _collection.InsertOneAsync(deletedAttachment, cancellationToken: cancellationToken);
                return true;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(DeletedAttachmentKey key, CancellationToken cancellationToken)
        {
            DeleteResult result = await _collection.DeleteOneAsync(x => x.Id == key, cancellationToken);
            return result.DeletedCount > 0;
        }

        public async Task DeleteByChatAsync(DeletedChatKey chat, CancellationToken cancellationToken)
        {
            await _collection.DeleteManyAsync(x => x.Id.ChatType == chat.ChatType && x.Id.ChatId == chat.ChatId, cancellationToken);
        }
    }
}
