using Application.Models.Output;

namespace Application.Services.Interfaces
{
    public interface IBotUserCacheService
    {
        public Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken);
        public Task<Bot?> GetBotAsync(Guid id, CancellationToken cancellationToken);
        public Task SaveUserAsync(User user, CancellationToken cancellationToken);
        public Task SaveBotAsync(Bot bot, CancellationToken cancellationToken);
        public Task InvalidateAsync(Guid id, CancellationToken cancellationToken);
    }
}
