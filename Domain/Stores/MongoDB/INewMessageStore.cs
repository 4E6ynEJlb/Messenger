using Domain.Models.Documents;
using Domain.Models.Documents.Keys;
using Domain.Models.Types;

namespace Domain.Stores.MongoDB
{
    public interface INewMessageStore
    {
        public Task<NewMessage?> GetOneByIdAsync(Guid messageId, CancellationToken cancellationToken);
        public Task<NewMessage?> GetOneEldestAsync(CancellationToken cancellationToken);
        public Task<List<NewMessage>> GetListByIdAsync(EnChatType chatType, Guid chatId, Guid[] ids, CancellationToken cancellationToken);
        public Task<List<NewMessage>> GetListByChatAsync(EnChatType chatType, Guid chatId, DateTime sentBefore, uint count, CancellationToken cancellationToken);
        public Task<bool> SaveAsync(NewMessage message, CancellationToken cancellationToken);
        public Task<bool> SaveManyAsync(List<NewMessage> messages, CancellationToken cancellationToken);
        public Task<bool> UpdateAsync(NewMessage message, CancellationToken cancellationToken);
        public Task DeleteByChatAsync(DeletedChatKey chat, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
