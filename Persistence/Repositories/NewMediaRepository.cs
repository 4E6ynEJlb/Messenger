using Domain.Models.Documents;
using Domain.Stores.MongoDB;
using Infrastructure.Database;
using MongoDB.Bson;
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

        public async Task<List<NewMedia>> GetManyByIdAsync(List<Guid> ids, CancellationToken cancellationToken)
        {
            return await _collection.Find(x => ids.Contains(x.MediaId))
                .ToListAsync(cancellationToken);
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

        public async Task IncrementMediaLinksAsync(List<Guid> id, CancellationToken cancellationToken)
        {
            UpdateDefinition<NewMedia> update = Builders<NewMedia>.Update.Inc(m => m.LinksCount, 1);
            FilterDefinition<NewMedia> filter = Builders<NewMedia>.Filter.In(m => m.MediaId, id);
            await _collection.UpdateManyAsync(filter, update, cancellationToken: cancellationToken);
        }

        public async Task DecrementLinksAsync(Dictionary<Guid, int> decrements, CancellationToken cancellationToken)
        {
            List<Guid> ids = decrements.Keys.ToList();

            FilterDefinition<NewMedia> filter = Builders<NewMedia>.Filter.In(m => m.MediaId, ids);

            BsonDocument decrementDocument = new BsonDocument(
                decrements.Select(x => new BsonElement(x.Key.ToString(), x.Value)));

            PipelineDefinition<NewMedia, NewMedia> update = new EmptyPipelineDefinition<NewMedia>()
                .AppendStage<NewMedia, NewMedia, NewMedia>(
                    new BsonDocument("$set",
                        new BsonDocument(
                            "LinksCount",
                            new BsonDocument("$subtract",
                                new BsonArray
                                {
                                    "$LinksCount",
                                    new BsonDocument("$ifNull",
                                    new BsonArray
                                    {
                                        new BsonDocument("$getField",
                                        new BsonDocument
                                        {
                                            { "field", "$MediaId" },
                                            { "input", decrementDocument }
                                        }),
                                        0
                                    })
                                }))));

            await _collection.UpdateManyAsync(filter, update, cancellationToken: cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            DeleteResult result = await _collection.DeleteOneAsync(x => x.MediaId == id, cancellationToken);
            return result.DeletedCount > 0;
        }

        public async Task DeleteManyAsync(List<Guid> id, CancellationToken cancellationToken)
        {
            await _collection.DeleteManyAsync(x => id.Contains(x.MediaId), cancellationToken);
        }
    }
}
