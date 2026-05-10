using Application.Models.Input;
using Application.Models.Internal.Constants;
using Application.Models.Output;
using Domain.Models;
using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Extensions;
using UserAPI.Models;

namespace UserAPI.Controllers
{
    [Authorize(Policies.USER_POLICY)]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        string mediaPrefix = "https://localhost:10102/Media";
        private readonly IUserStore _userStore;
        private readonly IObjectStorage _objectStorage;
        public UserController(IUserStore userStore, IObjectStorage objectStorage)
        {
            _userStore = userStore;
            _objectStorage = objectStorage;
        }
        /// <summary>
        /// current user info by id from jwt
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
            var userId = HttpContext.GetUserId();
            var user = await _userStore.GetUserByIdAsync(userId, cancellationToken);
            User u = new User
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Tag = user.Tag,
                Avatar = user.Avatar is not null ? $"{mediaPrefix}/{user.Avatar}" : null,
                BirthDate = user.BirthDate,
                Bio = user.Bio,
                WasOnline = user.WasOnline
            };
            return Ok(u);
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
            var user = await _userStore.GetUserByIdAsync(userId, cancellationToken);
            User u = new User
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Tag = user.Tag,
                Avatar = $"{mediaPrefix}/{user.Avatar}",
                BirthDate = user.BirthDate,
                Bio = user.Bio,
                WasOnline = user.WasOnline
            };
            return Ok(u);
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
            var user = await _userStore.GetUserByTagAsync(tag, cancellationToken);
            User u = new User
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Tag = user.Tag,
                Avatar = $"{mediaPrefix}/{user.Avatar}",
                BirthDate = user.BirthDate,
                Bio = user.Bio,
                WasOnline = user.WasOnline
            };
            return Ok(u);
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
            Guid[] avatars = await _userStore.GetUserAvatarsAsync(HttpContext.GetUserId(), cancellationToken);
            string[] mediaLinks = avatars.Select(a => $"{mediaPrefix}/{a}").ToArray();
            return Ok(mediaLinks);
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
            Guid[] avatars = await _userStore.GetUserAvatarsAsync(userId, cancellationToken);
            string[] mediaLinks = avatars.Select(a => $"{mediaPrefix}/{a}").ToArray();
            return Ok(mediaLinks);
        }

        /// <summary>
        /// use if current user is banned
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
            var userId = HttpContext.GetUserId();
            var banInfo = await _userStore.GetBannedUserInformationAsync(userId, cancellationToken);
            if (banInfo is null)
                return NotFound();
            BanInformation banInformation = new BanInformation
            {
                UserId = banInfo.UserId,
                BannedBy = banInfo.BannedBy,
                Reason = banInfo.Reason,
                BannedAt = banInfo.BannedAt,
                UnbannedAt = banInfo.UnbannedAt
            };
            return Ok(banInformation);
        }

        /// <summary>
        /// 
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
            var userId = HttpContext.GetUserId();
            var chats = await _userStore.GetUserChatsAsync(userId, pageOptions.Page, pageOptions.PageSize, cancellationToken);
            var chatsShortInfo = chats.Select(c => new ChatShortInfo
            {
                ChatId = c.ChatId,
                ChatType = c.ChatType switch
                {
                    EnChatType.Personal => ChatType.Personal,
                    EnChatType.Public => ChatType.Group,
                    EnChatType.Bot => ChatType.Bot,
                    _ => throw new Exception("invalid chat type")
                },
                ChatName = c.ChatName,
                NewMessagesCount = c.NewMessagesCount,
                ChatImage = c.ChatImage is not null ? $"{mediaPrefix}/{c.ChatImage}" : null
            }).ToArray();
            return Ok(chatsShortInfo);
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
            var userId = HttpContext.GetUserId();
            var updateUserAuthModel = new UpdateUserAuthModel
            {
                UserId = userId,
                UserCurrentPassword = updateCredentials.OldPassword,
                UserNewLogin = updateCredentials.Login,
                UserNewPassword = updateCredentials.Password
            };
            await _userStore.UpdateUserAuthAsync(updateUserAuthModel, cancellationToken);
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
            var userId = HttpContext.GetUserId();
            UserData updateUserData = new UserData
            {
                UserId = userId,
                FirstName = updateUser.FirstName,
                LastName = updateUser.LastName,
                Tag = updateUser.Tag,
                BirthDate = updateUser.BirthDate,
                Bio = updateUser.Bio,
                Avatar = null,
                WasOnline = DateTime.UtcNow
            };
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
            var userId = HttpContext.GetUserId();
            Guid mediaId = Guid.NewGuid();
            await _userStore.UploadUserAvatarAsync(userId, new MediaFile
            {
                MediaId = mediaId,
                FileName = avatar.FileName,
                ContentType = avatar.ContentType
            }, cancellationToken);
            await _objectStorage.SaveAsync(avatar.OpenReadStream(), mediaId, cancellationToken);
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
            var userId = HttpContext.GetUserId();
            if (!mediaLink.StartsWith(mediaPrefix))
                return BadRequest();
            await _userStore.DeleteUserAvatarAsync(userId, Guid.Parse(mediaLink[mediaPrefix.Length..]), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// deletes current user's account
        /// </summary>
        /// <param name="password">manually entered encrypted password</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAccount(string password, CancellationToken cancellationToken)
        {
            Response.DeleteRefreshToken();
            Response.DeleteDeviceId();
            Response.DeleteUserId();
            var userId = HttpContext.GetUserId();
            await _userStore.DeleteUserAsync(userId, password, cancellationToken);
            return Ok();
        }
    }
}
