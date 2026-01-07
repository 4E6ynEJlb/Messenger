using Application.Models.Input;
using UserAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace UserAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="isRemembering">true = longer expiration</param>
        /// <returns>jwt in response, refresh in cookies</returns>
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login(UserCredentials credentials, bool isRemembering)
        {
            if (isRemembering)
                Response.AppendCredentials("credentials");
            Response.AppendRefreshToken("token", 1);
            return Ok();//jwt token
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerUser"></param>
        /// <param name="isRemembering">true = longer expiration</param>
        /// <returns>jwt in response, refresh in cookies</returns>
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Register(RegisterUser registerUser, bool isRemembering)
        {
            if (isRemembering)
                Response.AppendCredentials("credentials");
            Response.AppendRefreshToken("token", 1);
            return Ok();//jwt token
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>jwt in response, refresh in cookies</returns>
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(401)]
        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken()
        {
            Request.TryGetRefreshToken();
            Response.AppendRefreshToken("token", 1);
            return Ok();//jwt token
        }

        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(401)]
        [HttpPost("[action]")]
        public async Task<IActionResult> AutoRelogin()
        {
            Request.TryGetCredentials();
            Response.AppendRefreshToken("token", 1);
            return Ok();//jwt token
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            Response.DeleteRefreshToken();
            Response.DeleteCredentials();
            return Ok();
        }
    }
}
