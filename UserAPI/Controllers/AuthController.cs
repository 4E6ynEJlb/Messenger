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
        [ProducesResponseType(typeof(string), 302)]
        [HttpPost("[action]")]
        public IActionResult Login(UserCredentials credentials, bool isRemembering)
        {
            Response.AppendRefreshToken();//refresh token
            return this.NewJwtToken();//jwt token
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerUser"></param>
        /// <param name="isRemembering">true = longer expiration</param>
        /// <returns>jwt in response, refresh in cookies</returns>
        [ProducesResponseType(typeof(string), 200)]
        [HttpPost("[action]")]
        public IActionResult Register(RegisterUser registerUser, bool isRemembering)
        {
            Response.AppendRefreshToken();//refresh token
            return this.NewJwtToken();//jwt token
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>jwt in response, refresh in cookies</returns>
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 302)]
        [HttpPost("[action]")]
        public IActionResult RefreshToken()
        {
            Request.TryGetRefreshToken();//get refresh token from cookie
            Response.AppendRefreshToken();//refresh token
            return this.NewJwtToken();//jwt token
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>no fuck</returns>
        [ProducesResponseType(200)]
        [HttpPost("[action]")]
        public IActionResult Logout()
        {
            Request.TryGetRefreshToken();//get refresh token from cookie
            return Ok();
        }
    }
}
