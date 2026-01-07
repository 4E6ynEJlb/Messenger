using Application.Models.Input;
using Application.Models.Output;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Extensions;

namespace UserAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// current user info by id from jwt
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> Current()
        {
            return Ok();
        }

        /// <summary>
        /// user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            return Ok();
        }

        /// <summary>
        /// user by tag with exact match
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]/{tag}")]
        public async Task<IActionResult> Search(string tag)
        {
            return Ok();
        }

        /// <summary>
        /// avatars of current user; first is main
        /// </summary>
        /// <returns>media links</returns>
        [ProducesResponseType(typeof(string[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetCurrentUserAvatars()
        {
            return Ok();
        }

        /// <summary>
        /// avatars of user by id; first is main
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>media links</returns>
        [ProducesResponseType(typeof(string[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetUserAvatars(Guid userId)
        {
            return Ok();
        }

        /// <summary>
        /// use if current user is banned
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(BanInformation), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetBanInformation()
        {
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatsSelectOptions">all options values >= 1</param>
        /// <returns>part of chats list</returns>
        [ProducesResponseType(typeof(ChatShortInfo[]), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Chats(ChatsSelectOptions chatsSelectOptions)
        {
            return Ok();
        }

        /// <summary>
        /// updates current user credentials
        /// </summary>
        /// <param name="updateCredentials"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateCredentials(UpdateCredentials updateCredentials)
        {
            Response.AppendCredentials("credentials");
            return Ok();
        }

        /// <summary>
        /// updates current user profile
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateProfile(UpdateUser updateUser)
        {
            return Ok();
        }

        /// <summary>
        /// saves new main avatar for current user
        /// </summary>
        /// <param name="avatar">png, bmp, jpeg, gif, tiff, webp</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            return Ok();
        }

        /// <summary>
        /// deletes current user's avatar by media link
        /// </summary>
        /// <param name="mediaLink">avatar url</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAvatar(string mediaLink)
        {
            return Ok();
        }

        /// <summary>
        /// deletes current user's account
        /// </summary>
        /// <param name="password">manually entered encrypted password</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAccount(byte[] password)
        {
            Response.DeleteRefreshToken();
            Response.DeleteCredentials();
            return Ok();
        }
    }
}
