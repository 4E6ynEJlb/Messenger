using Domain.Models.Documents;
using Domain.Models.Types;

namespace Domain.Stores.MongoDB
{
    public interface INewMessageStore
    {
        public Task<NewMessage?> GetOneByIdAsync(Guid messageId, CancellationToken cancellationToken);
        public Task<NewMessage?> GetOneEldestAsync(CancellationToken cancellationToken);
        public Task<List<NewMessage>> GetListByChatAsync(EnChatType chatType, Guid chatId, DateTime sentBefore, int count, CancellationToken cancellationToken);
        public Task<bool> CreateAsync(NewMessage message, CancellationToken cancellationToken);
        public Task<bool> UpdateAsync(NewMessage message, CancellationToken cancellationToken);
        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
