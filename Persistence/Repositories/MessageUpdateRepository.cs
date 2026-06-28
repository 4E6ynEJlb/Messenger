using Domain.Models.Documents;
using Domain.Stores.MongoDB;
using Infrastructure.Database;
using MongoDB.Driver;

namespace Persistence.Repositories
{
    public class MessageUpdateRepository : IMessageUpdateStore
    {
        private readonly IMongoCollection<PublicMessageUpdate> _collection;

        public MessageUpdateRepository(UpdatesContext context)
        {
            _collection = context.MessageUpdates;
        }

        public async Task<PublicMessageUpdate?> GetOneEldestAsync(CancellationToken cancellationToken)
        {
            return await _collection.Find(FilterDefinition<PublicMessageUpdate>.Empty)
                .SortBy(x => x.UpdatedAt).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> CreateAsync(PublicMessageUpdate messageUpdate, CancellationToken cancellationToken)
        {
            try
            {
                await _collection.InsertOneAsync(messageUpdate, cancellationToken: cancellationToken);
                return true;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            DeleteResult result = await _collection.DeleteOneAsync(x => x.UpdateId == id, cancellationToken);
            return result.DeletedCount > 0;
        }

        public async Task DeleteByChatAsync(Guid chat, CancellationToken cancellationToken)
        {
            await _collection.DeleteManyAsync(x => x.ChatId == chat, cancellationToken);
        }
    }
}
