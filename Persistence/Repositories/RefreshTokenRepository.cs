using Domain.Stores;
using Infrastructure.Database;

namespace Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenStore
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public RefreshTokenRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<string> CreateRefreshTokenAsync(Guid userId, TimeSpan lifetime, Guid deviceId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateRefreshTokenAsync(Guid userId, string oldToken, Guid deviceId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateRefreshTokenAsync(string refreshToken, Guid deviceId, Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
