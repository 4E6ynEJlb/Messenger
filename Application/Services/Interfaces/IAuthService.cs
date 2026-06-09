using Application.Models.Input;
using Application.Models.Internal;

namespace Application.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<LoginResult> LoginAsync(UserCredentials credentials, bool IsRemembering, CancellationToken cancellationToken);
        public Task<LoginResult> RegisterAsync(RegisterUser registerUser, bool isRemembering, CancellationToken cancellationToken);
        public Task<TokenValidationResult?> ValidateTokenAsync(string token, Guid deviceId, Guid userId, CancellationToken cancellationToken);
        public Task InvalidateTokenAsync(string token, CancellationToken cancellationToken);
    }
}
