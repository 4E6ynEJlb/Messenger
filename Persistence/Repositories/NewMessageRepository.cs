using Domain.Models.Documents;
using Domain.Models.Types;
using Domain.Stores.MongoDB;
using Infrastructure.Database;
using MongoDB.Driver;

namespace Persistence.Repositories
{
    public class NewMessageRepository : INewMessageStore
    {
        private readonly IMongoCollection<NewMessage> _collection;

        public NewMessageRepository(UpdatesContext context)
        {
            _collection = context.NewMessages;
        }

        public async Task<NewMessage?> GetOneByIdAsync(Guid messageId, CancellationToken cancellationToken)
        {
            return await _collection.Find(x => x.MessageId == messageId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<NewMessage?> GetOneEldestAsync(CancellationToken cancellationToken)
        {
            return await _collection.Find(FilterDefinition<NewMessage>.Empty)
                .SortBy(x => x.SentAt).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<NewMessage>> GetListByIdAsync(EnChatType chatType, Guid chatId, Guid[] ids, CancellationToken cancellationToken)
        {
            return await _collection.Find(x =>
                x.ChatType == chatType &&
                x.ChatId == chatId &&
                ids.Contains(x.MessageId)).SortByDescending(x => x.SentAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<NewMessage>> GetListByChatAsync(EnChatType chatType, Guid chatId, DateTime sentBefore, uint count, CancellationToken cancellationToken)
        {
            return await _collection.Find(x =>
                x.ChatType == chatType &&
                x.ChatId == chatId &&
                x.SentAt < sentBefore).SortByDescending(x => x.SentAt)
            .Limit((int)count).ToListAsync(cancellationToken);
        }

        public async Task<bool> SaveAsync(NewMessage message, CancellationToken cancellationToken)
        {
            try
            {
                await _collection.InsertOneAsync(message, cancellationToken: cancellationToken);
                return true;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return false;
            }
        }

        public async Task<bool> SaveManyAsync(List<NewMessage> messages, CancellationToken cancellationToken)
        {
            try
            {
                await _collection.InsertManyAsync(messages, cancellationToken: cancellationToken);
                return true;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(NewMessage message, CancellationToken cancellationToken)
        {
            ReplaceOneResult result = await _collection.ReplaceOneAsync(x => x.MessageId == message.MessageId, 
                message, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            DeleteResult result = await _collection.DeleteOneAsync(x => x.MessageId == id, cancellationToken);
            return result.DeletedCount > 0;
        }
    }
}
