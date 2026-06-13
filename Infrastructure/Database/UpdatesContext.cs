using Domain.Models.Documents;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infrastructure.Database
{
    public class UpdatesContext
    {
        public IMongoDatabase Database { get; }
        public IMongoCollection<NewMessage> NewMessages { get; }
        public IMongoCollection<DeletedMessage> DeletedMessages { get; }
        public IMongoCollection<DeletedAttachment> DeletedAttachments { get; }
        public IMongoCollection<NewMedia> NewMedia { get; }
        public IMongoCollection<PublicMessageUpdate> MessageUpdates { get; }

        public UpdatesContext(IMongoClient client, IConfiguration configuration)
        {
            string databaseName =
                configuration.GetSection("Mongo")["Database"]
                ?? throw new ArgumentNullException("Mongo database");

            Database = client.GetDatabase(databaseName);

            NewMessages = Database.GetCollection<NewMessage>("new_messages");
            DeletedMessages = Database.GetCollection<DeletedMessage>("deleted_messages");
            DeletedAttachments = Database.GetCollection<DeletedAttachment>("deleted_attachments");
            NewMedia = Database.GetCollection<NewMedia>("new_media");
            MessageUpdates = Database.GetCollection<PublicMessageUpdate>("message_updates");
        }

        public async Task InitializeAsync()
        {
            await CreateIndexesAsync();
        }

        private async Task CreateIndexesAsync()
        {
            await NewMessages.Indexes.CreateManyAsync(
            [
                new CreateIndexModel<NewMessage>(
                Builders<NewMessage>.IndexKeys
                    .Ascending(x => x.ChatType)
                    .Ascending(x => x.ChatId)
                    .Descending(x => x.SentAt)
            )
            ]);

            await DeletedMessages.Indexes.CreateOneAsync(
                new CreateIndexModel<DeletedMessage>(
                    Builders<DeletedMessage>.IndexKeys
                        .Descending(x => x.DeletedAt)
                ));

            await DeletedAttachments.Indexes.CreateOneAsync(
                new CreateIndexModel<DeletedAttachment>(
                    Builders<DeletedAttachment>.IndexKeys
                        .Descending(x => x.DeletedAt)
                ));

            await MessageUpdates.Indexes.CreateOneAsync(
                new CreateIndexModel<PublicMessageUpdate>(
                    Builders<PublicMessageUpdate>.IndexKeys
                        .Descending(x => x.UpdatedAt)
                ));
        }
    }
}
