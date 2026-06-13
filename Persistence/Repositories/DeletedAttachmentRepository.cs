using Domain.Models.Documents;
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

        public async Task<bool> CheckDeletionByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _collection.Find(x => x.MediaId == id)
                .AnyAsync(cancellationToken);
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

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            DeleteResult result = await _collection.DeleteOneAsync(x => x.MediaId == id, cancellationToken);
            return result.DeletedCount > 0;
        }
    }
}
