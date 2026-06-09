using Application.Models.Output;

namespace Application.Services.Interfaces
{
    public interface IMessageCacheService
    {
        public Task<Message?> GetAsync(Guid messageId, Guid chatId, CancellationToken cancellationToken);
        public Task SaveAsync(Message message, CancellationToken cancellationToken);
        public Task InvalidateAsync(Guid messageId, Guid chatId, CancellationToken cancellationToken);
    }
}
