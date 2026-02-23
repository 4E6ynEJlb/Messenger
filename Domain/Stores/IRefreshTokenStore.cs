namespace Domain.Stores
{
    public interface IRefreshTokenStore///////////impl
    {
        public Task<bool> ValidateRefreshTokenAsync(string refreshToken, string deviceId);
        public Task<string> CreateRefreshTokenAsync(Guid userId, DateTime expiration, string deviceId);
        public Task<string> UpdateRefreshTokenAsync(Guid userId, string oldToken, DateTime expiration, string deviceId);
        public Task InvalidateRefreshTokenAsync(Guid userId, string refreshToken);
    }
}
