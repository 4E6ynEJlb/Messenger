using Domain.Models.Documents;
using Domain.Models.Documents.Keys;
using Domain.Models.Types;
using Domain.Stores.MongoDB;
using Infrastructure.Database;
using MongoDB.Driver;

namespace Persistence.Repositories
{
    public class DeletedMessageRepository : IDeletedMessageStore
    {
        private readonly IMongoCollection<DeletedMessage> _collection;

        public DeletedMessageRepository(UpdatesContext context)
        {
            _collection = context.DeletedMessages;
        }

        public async Task<DeletedMessage?> GetOneEldestAsync(CancellationToken cancellationToken)
        {
            return await _collection.Find(FilterDefinition<DeletedMessage>.Empty)
                .SortBy(x => x.DeletedAt).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> CheckDeletionByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _collection.Find(x => x.MessageId == id)
                .AnyAsync(cancellationToken);
        }

        public async Task<uint> GetDeletedCountAsync(Guid chatId, EnChatType chatType, DateTime sentBefore, CancellationToken cancellationToken)
        {
            return (uint)await _collection.CountDocumentsAsync(x => x.ChatId == chatId && x.ChatType == chatType && x.DeletedAt < sentBefore, cancellationToken: cancellationToken);
        }

        public async Task<bool> CreateAsync(DeletedMessage deletedMessage, CancellationToken cancellationToken)
        {
            try
            {
                await _collection.InsertOneAsync(deletedMessage, cancellationToken: cancellationToken);
                return true;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            DeleteResult result = await _collection.DeleteOneAsync(x => x.MessageId == id, cancellationToken);
            return result.DeletedCount > 0;
        }

        public async Task DeleteByChatAsync(DeletedChatKey chat, CancellationToken cancellationToken)
        {
            await _collection.DeleteManyAsync(x => x.ChatType == chat.ChatType && x.ChatId == chat.ChatId, cancellationToken);
        }
    }
}
