using Application.Models.Input;
using Application.Models.Output;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(User), 200)]
        public IActionResult Current()
        {
            return Ok();
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(User), 200)]
        public IActionResult GetUserById(Guid userId)
        {
            return Ok();
        }

        [HttpGet("[action]/{tag}")]
        [ProducesResponseType(typeof(User), 200)]
        public IActionResult Search(string tag)
        {
            return Ok();
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(string[]), 200)]
        public IActionResult GetCurrentUserAvatars()
        {
            return Ok();
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(string[]), 200)]
        public IActionResult GetUserAvatars(Guid userId)
        {
            return Ok();
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(BanInformation), 200)]
        public IActionResult GetBanInformation()
        {
            return Ok();
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(ChatShortInfo[]), 200)]
        public IActionResult Chats()
        {
            return Ok();
        }

        [HttpPost("[action]")]
        [ProducesResponseType(200)]
        public IActionResult UpdateCredentials(UpdateCredentials updateCredentials)
        {
            return Ok();
        }

        [HttpPost("[action]")]
        [ProducesResponseType(200)]
        public IActionResult UpdateProfile(UpdateUser updateUser)
        {
            return Ok();
        }

        [HttpPost("[action]")]
        [ProducesResponseType(200)]
        public IActionResult UploadAvatar(IFormFile avatar)
        {
            return Ok();
        }

        [HttpDelete("[action]")]
        [ProducesResponseType(200)]
        public IActionResult DeleteAvatar(Guid mediaId)
        {
            return Ok();
        }

        [HttpDelete("[action]")]
        [ProducesResponseType(200)]
        public IActionResult DeleteAccount(byte[] password)
        {
            return Ok();
        }
    }
}
