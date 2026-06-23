using Domain.Models.Documents;
using Domain.Models.Documents.Keys;
using Domain.Models.Types;
using Domain.Stores.MongoDB;
using Infrastructure.Database;
using MongoDB.Driver;

namespace Persistence.Repositories
{
    public class DeletedChatRepository : IDeletedChatStore
    {
        private readonly IMongoCollection<DeletedChat> _collection;
        public DeletedChatRepository(UpdatesContext updatesContext)
        {
            _collection = updatesContext.DeletedChats;
        }

        public async Task<DeletedChat?> GetOneEldestAsync(CancellationToken cancellationToken)
        {
            return await _collection.Find(FilterDefinition<DeletedChat>.Empty)
                .SortBy(x => x.DeletedAt).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task SaveAsync(Guid chatId, EnChatType chatType, CancellationToken cancellationToken)
        {
            try
            {
                await _collection.InsertOneAsync(new DeletedChat
                {
                    Id = new DeletedChatKey
                    {
                        ChatId = chatId,
                        ChatType = chatType
                    },
                    DeletedAt = DateTime.UtcNow
                }, cancellationToken: cancellationToken);
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            { }
        }

        public async Task DeleteAsync(Guid chatId, EnChatType chatType, CancellationToken cancellationToken)
        {
            await _collection.DeleteOneAsync(x => x.Id.ChatId == chatId && x.Id.ChatType == chatType, cancellationToken);
        }
    }
}
