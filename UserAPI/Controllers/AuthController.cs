using Application.Models.Input;
using Application.Models.Internal;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Extensions;
using UserAPI.Models;

namespace UserAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        /// <summary>
        /// After call check user role in jwt. If user is banned, role will be "BannedUser", else "User".
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="isRemembering">true = longer expiration</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>jwt in response, refresh in cookies</returns>
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login(UserCredentials credentials, bool isRemembering, CancellationToken cancellationToken)
        {
            LoginResult result = await _authService.LoginAsync(credentials, isRemembering, cancellationToken);
            string role = result.IsBanned ? Roles.BANNED_USER : Roles.USER;
            Response.AppendExpiration((int)result.ExpirationHours);
            Response.AppendDeviceId(result.DeviceId, (int)result.ExpirationHours);
            Response.AppendUserId(result.UserId, (int)result.ExpirationHours);
            Response.AppendRefreshToken(result.RefreshToken, (int)result.ExpirationHours);
            return Ok(JwtHelper.GenerateJwtToken(result.UserId, role));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerUser"></param>
        /// <param name="isRemembering">true = longer expiration</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>jwt in response, refresh in cookies</returns>
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Register(RegisterUser registerUser, bool isRemembering, CancellationToken cancellationToken)
        {
            LoginResult result = await _authService.RegisterAsync(registerUser, isRemembering, cancellationToken);
            string role = result.IsBanned ? Roles.BANNED_USER : Roles.USER;
            Response.AppendExpiration((int)result.ExpirationHours);
            Response.AppendDeviceId(result.DeviceId, (int)result.ExpirationHours);
            Response.AppendUserId(result.UserId, (int)result.ExpirationHours);
            Response.AppendRefreshToken(result.RefreshToken, (int)result.ExpirationHours);
            return Ok(JwtHelper.GenerateJwtToken(result.UserId, role));
        }

        /// <summary>
        /// Use for getting new jwt when the old one is expired. After call check user role in jwt. If user is banned, role will be "BannedUser", else "User"
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>jwt in response, refresh in cookies</returns>
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(401)]
        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            string? token = Request.TryGetRefreshToken();
            Guid? deviceId = Request.TryGetDeviceId();
            Guid? userId = Request.TryGetUserId();
            int? expiration = Request.TryGetExpiration() ?? 1;
            if (token is null || deviceId is null || userId is null)
                return Unauthorized();
            TokenValidationResult? result = await _authService.ValidateTokenAsync(token, deviceId.Value, userId.Value, cancellationToken);
            if (result is null)
                return Unauthorized();
            Response.AppendExpiration(expiration.Value);
            Response.AppendDeviceId(deviceId.Value, expiration.Value);
            Response.AppendUserId(userId.Value, expiration.Value);
            Response.AppendRefreshToken(result.Token, expiration.Value);
            string role = result.IsBanned ? Roles.BANNED_USER : Roles.USER;
            return Ok(JwtHelper.GenerateJwtToken(userId.Value, role));
        }
        
        /// <summary>
        /// After call delete jwt token and go to auth page
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            string? token = Request.TryGetRefreshToken();
            if (token is not null)
                await _authService.InvalidateTokenAsync(token, cancellationToken);
            Response.DeleteRefreshToken();
            Response.DeleteDeviceId();
            Response.DeleteUserId();
            Response.DeleteExpiration();
            return Ok();
        }
    }
}
