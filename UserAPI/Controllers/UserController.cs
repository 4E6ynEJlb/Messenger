using Application.Models.Input;
using Application.Models.Output;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Extensions;
using UserAPI.Models;

namespace UserAPI.Controllers
{
    /// <summary>
    /// Only for not banned users with some excepted endpoints
    /// </summary>
    [Authorize(Policies.USER_POLICY)]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {        
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        /// <summary>
        /// current user info by id from jwt. Available for banned and not banned users
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [Authorize(Policies.ANY_USER_POLICY)]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> Current(CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();            
            return Ok(await _userService.GetUserAsync(userId, cancellationToken));
        }

        /// <summary>
        /// user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId, CancellationToken cancellationToken)
        {
            return Ok(await _userService.GetUserAsync(userId, cancellationToken));
        }

        /// <summary>
        /// user by tag with exact match
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]/{tag}")]
        public async Task<IActionResult> Search(string tag, CancellationToken cancellationToken)
        {            
            return Ok(await _userService.GetUserByTagAsync(tag, cancellationToken));
        }

        /// <summary>
        /// avatars of current user; first is main
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(string[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetCurrentUserAvatars(CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _userService.GetAvatarsAsync(userId, cancellationToken));
        }

        /// <summary>
        /// avatars of user by id; first is main
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>media links</returns>
        [ProducesResponseType(typeof(string[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserAvatars(Guid userId, CancellationToken cancellationToken)
        {
            return Ok(await _userService.GetAvatarsAsync(userId, cancellationToken));
        }

        /// <summary>
        /// use if current user is banned. Only for banned users
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [Authorize(Policies.BANNED_USER_POLICY)]
        [ProducesResponseType(typeof(BanInformation), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetBanInformation(CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _userService.GetBanInformationAsync(userId, cancellationToken));
        }

        /// <summary>
        /// Call for chats list of current user
        /// </summary>
        /// <param name="pageOptions">all options values >= 1</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>part of chats list</returns>
        [ProducesResponseType(typeof(ChatShortInfo[]), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Chats(PageOptions pageOptions, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _userService.GetChatsAsync(userId, pageOptions, cancellationToken));
        }

        /// <summary>
        /// updates current user credentials
        /// </summary>
        /// <param name="updateCredentials"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateCredentials(UpdateCredentials updateCredentials, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _userService.UpdateCredentialsAsync(userId, updateCredentials, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// updates current user profile
        /// </summary>
        /// <param name="updateUser"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateProfile(UpdateUser updateUser, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _userService.UpdateProfileAsync(userId, updateUser, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// saves new main avatar for current user
        /// </summary>
        /// <param name="avatar">png, bmp, jpeg, gif, tiff, webp</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _userService.UploadAvatarAsync(userId, avatar.ToFileUpload(), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// deletes current user's avatar by media link
        /// </summary>
        /// <param name="mediaLink">avatar url</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAvatar(string mediaLink, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _userService.DeleteAvatarAsync(userId, mediaLink, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// deletes current user's account
        /// </summary>
        /// <param name="password">manually entered password</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAccount(string password, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _userService.DeleteAccountAsync(userId, password, cancellationToken);
            Response.DeleteRefreshToken();
            Response.DeleteDeviceId();
            Response.DeleteUserId();
            return Ok();
        }
    }
}
