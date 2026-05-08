namespace Domain.Stores
{
    public interface IRefreshTokenStore
    {
        public Task<bool> ValidateRefreshTokenAsync(string refreshToken, Guid deviceId, Guid userId);
        public Task<string> CreateRefreshTokenAsync(Guid userId, TimeSpan lifetime, Guid deviceId);
        public Task<string> UpdateRefreshTokenAsync(Guid userId, string oldToken, Guid deviceId);
        public Task InvalidateRefreshTokenAsync(string refreshToken);
    }
}
