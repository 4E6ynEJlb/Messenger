using Application.Models.Input;
using UserAPI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Domain.Stores;
using UserAPI.Models;
using Domain.Models;

namespace UserAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserStore _userStore;
        private readonly IRefreshTokenStore _refreshTokenStore;
        public AuthController(IUserStore userStore, IRefreshTokenStore refreshTokenStore)
        {
            _userStore = userStore;
            _refreshTokenStore = refreshTokenStore;
        }
        /// <summary>
        /// 
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
            var u = await _userStore.AuthUserAsync(credentials.Login, credentials.Password, cancellationToken);
            Guid deviceId = Guid.NewGuid();
            int hours = isRemembering ? 31 * 24 : 1;
            Response.AppendExpiration(hours);
            Response.AppendDeviceId(deviceId, hours);
            Response.AppendUserId(u.UserId, hours);
            string r = await _refreshTokenStore.CreateRefreshTokenAsync(u.UserId, TimeSpan.FromHours(hours), deviceId, cancellationToken);
            Response.AppendRefreshToken(r, hours);
            bool b = await _userStore.CheckUserBanStatusAsync(u.UserId, cancellationToken);
            string role = b ? Roles.BANNED_USER : Roles.USER;
            return Ok(JwtHelper.GenerateJwtToken(u.UserId, role));
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
            RegisterUserModel registerUserModel = new RegisterUserModel
            {
                UserLogin = registerUser.UserLogin,
                UserPassword = registerUser.UserPassword,
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Tag = registerUser.Tag,
                BirthDate = registerUser.BirthDate
            };
            var id = await _userStore.RegisterUserAsync(registerUserModel, cancellationToken);
            Guid deviceId = Guid.NewGuid();
            int hours = isRemembering ? 31 * 24 : 1;
            Response.AppendExpiration(hours);
            Response.AppendDeviceId(deviceId, hours);
            Response.AppendUserId(id, hours);
            string r = await _refreshTokenStore.CreateRefreshTokenAsync(id, TimeSpan.FromHours(hours), deviceId, cancellationToken);
            Response.AppendRefreshToken(r, hours);
            return Ok(JwtHelper.GenerateJwtToken(id, Roles.USER));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>jwt in response, refresh in cookies</returns>
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(401)]
        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            var r = Request.TryGetRefreshToken();
            var d = Request.TryGetDeviceId();
            var u = Request.TryGetUserId();
            if (r == null || d == null || u == null)
                return Unauthorized();
            if (!await _refreshTokenStore.ValidateRefreshTokenAsync(r, d.Value, u.Value, cancellationToken))
                return Unauthorized();
            int? hours = Request.TryGetExpiration();
            if (hours == null)
                hours = 1;
            Response.AppendRefreshToken(await _refreshTokenStore.UpdateRefreshTokenAsync(u.Value, r, d.Value, cancellationToken), hours.Value);
            Response.AppendExpiration(hours.Value);
            Response.AppendDeviceId(d.Value, hours.Value);
            Response.AppendUserId(u.Value, hours.Value);
            bool ban = await _userStore.CheckUserBanStatusAsync(u.Value, cancellationToken);
            string role = ban ? Roles.BANNED_USER : Roles.USER;
            return Ok(JwtHelper.GenerateJwtToken(u.Value, role));
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var t = Request.TryGetRefreshToken();
            if (t != null)
                await _refreshTokenStore.InvalidateRefreshTokenAsync(t, cancellationToken);
            Response.DeleteRefreshToken();
            Response.DeleteDeviceId();
            Response.DeleteUserId();
            Response.DeleteExpiration();
            return Ok();
        }
    }
}
