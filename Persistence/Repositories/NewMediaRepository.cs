using Domain.Models.Documents;
using Domain.Stores.MongoDB;
using Infrastructure.Database;
using MongoDB.Driver;

namespace Persistence.Repositories
{
    public class NewMediaRepository : INewMediaStore
    {
        private readonly IMongoCollection<NewMedia> _collection;

        public NewMediaRepository(UpdatesContext context)
        {
            _collection = context.NewMedia;
        }

        public async Task<NewMedia?> GetOneByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _collection.Find(x => x.MediaId == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> CreateAsync(NewMedia newMedia, CancellationToken cancellationToken)
        {
            try
            {
                await _collection.InsertOneAsync(newMedia, cancellationToken: cancellationToken);
                return true;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            DeleteResult result = await _collection.DeleteOneAsync(x => x.MediaId == id, cancellationToken);
            return result.DeletedCount > 0;
        }
    }
}
