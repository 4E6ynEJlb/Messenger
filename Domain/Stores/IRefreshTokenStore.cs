namespace Domain.Stores
{
    public interface IRefreshTokenStore
    {
        public Task<bool> ValidateRefreshTokenAsync(string refreshToken, Guid deviceId, Guid userId, CancellationToken cancellationToken);
        public Task<string> CreateRefreshTokenAsync(Guid userId, TimeSpan lifetime, Guid deviceId, CancellationToken cancellationToken);
        public Task<string> UpdateRefreshTokenAsync(Guid userId, string oldToken, Guid deviceId, CancellationToken cancellationToken);
        public Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    }
}
