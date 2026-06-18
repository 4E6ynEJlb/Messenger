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
        public IMongoCollection<DeletedChat> DeletedChats { get; }

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
            DeletedChats = Database.GetCollection<DeletedChat>("deleted_chats");
        }

        public async Task InitializeAsync()
        {
            await CreateIndexesAsync();
        }

        private async Task CreateIndexesAsync()
        {
            await NewMessages.Indexes.CreateOneAsync(
                new CreateIndexModel<NewMessage>(
                Builders<NewMessage>.IndexKeys
                    .Ascending(x => x.ChatType)
                    .Ascending(x => x.ChatId)
                    .Ascending(x => x.SentAt)
            ));

            await DeletedMessages.Indexes.CreateManyAsync(
                [
                    new CreateIndexModel<DeletedMessage>(
                        Builders<DeletedMessage>.IndexKeys
                            .Ascending(x => x.DeletedAt)),
                    new CreateIndexModel<DeletedMessage>(
                        Builders<DeletedMessage>.IndexKeys
                            .Ascending(x => x.SentAt)
                            .Ascending(x => x.ChatType)
                            .Ascending(x => x.ChatId))
                ]
            );

            await DeletedAttachments.Indexes.CreateManyAsync(
                [
                    new CreateIndexModel<DeletedAttachment>(
                        Builders<DeletedAttachment>.IndexKeys
                            .Ascending(x => x.DeletedAt)),
                    new CreateIndexModel<DeletedAttachment>(
                        Builders<DeletedAttachment>.IndexKeys
                            .Ascending(x => x.Id.ChatType)
                            .Ascending(x => x.Id.ChatId)
                            .Ascending(x => x.Id.MessageId))
                ]
            );

            await MessageUpdates.Indexes.CreateOneAsync(
                new CreateIndexModel<PublicMessageUpdate>(
                    Builders<PublicMessageUpdate>.IndexKeys
                        .Ascending(x => x.UpdatedAt)
                ));

            await DeletedChats.Indexes.CreateOneAsync(
                new CreateIndexModel<DeletedChat>(
                    Builders<DeletedChat>.IndexKeys
                        .Ascending(x => x.DeletedAt)
                ));
        }
    }
}
