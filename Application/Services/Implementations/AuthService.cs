using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Internal.Options;
using Application.Services.Interfaces;
using Domain.Models;
using Domain.Models.Types;
using Domain.Stores;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserStore _userStore;
        private readonly IRefreshTokenStore _refreshTokenStore;
        private readonly uint _ephemeralExpiration;
        private readonly uint _rememberingExpiration;
        public AuthService(IUserStore userStore, IRefreshTokenStore refreshTokenStore, IOptions<TokenExpirationOptions> options)
        {
            _userStore = userStore;
            _refreshTokenStore = refreshTokenStore;
            _ephemeralExpiration = options.Value.EphemeralHours;
            _rememberingExpiration = options.Value.RememberingHours;
        }

        public async Task InvalidateTokenAsync(string token, CancellationToken cancellationToken)
        {
            await _refreshTokenStore.InvalidateRefreshTokenAsync(token, cancellationToken);
        }

        public async Task<LoginResult> LoginAsync(UserCredentials credentials, bool isRemembering, CancellationToken cancellationToken)
        {
            UserData userData = await _userStore.AuthUserAsync(credentials.Login, credentials.Password, cancellationToken);
            bool ban = await _userStore.CheckUserBanStatusAsync(userData.UserId, cancellationToken);
            
            Guid device = Guid.NewGuid();
            uint expiration = isRemembering ? _rememberingExpiration : _ephemeralExpiration;
            
            string token = await _refreshTokenStore.CreateRefreshTokenAsync(
                userData.UserId, TimeSpan.FromHours(expiration), device, cancellationToken);
            
            return new LoginResult() { 
                UserId =  userData.UserId, 
                IsBanned = ban, 
                DeviceId = device, 
                ExpirationHours = expiration,
                RefreshToken = token
            };
        }

        public async Task<LoginResult> RegisterAsync(RegisterUser registerUser, bool isRemembering, CancellationToken cancellationToken)
        {
            RegisterUserModel registerUserModel = registerUser.ToRegisterUserModel();
            Guid userId = await _userStore.RegisterUserAsync(registerUserModel, cancellationToken);

            Guid device = Guid.NewGuid();
            uint expiration = isRemembering ? _rememberingExpiration : _ephemeralExpiration;

            string token = await _refreshTokenStore.CreateRefreshTokenAsync(
                userId, TimeSpan.FromHours(expiration), device, cancellationToken);

            return new LoginResult()
            {
                UserId = userId,
                IsBanned = false,
                DeviceId = device,
                ExpirationHours = expiration,
                RefreshToken = token
            };
        }

        public async Task<TokenValidationResult?> ValidateTokenAsync(string token, Guid deviceId, Guid userId, CancellationToken cancellationToken)
        {
            if (!await _refreshTokenStore.ValidateRefreshTokenAsync(token, deviceId, userId, cancellationToken))
                return null;
            bool ban = await _userStore.CheckUserBanStatusAsync(userId, cancellationToken);
            string newToken = await _refreshTokenStore.UpdateRefreshTokenAsync(userId, token, deviceId, cancellationToken);

            return new TokenValidationResult()
            {
                Token = newToken,
                IsBanned = ban
            };
        }
    }
}
