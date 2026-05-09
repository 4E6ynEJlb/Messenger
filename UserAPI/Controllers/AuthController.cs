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
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>jwt in response, refresh in cookies</returns>
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login(UserCredentials credentials, bool isRemembering, CancellationToken cancellationToken)
        {            
            if (isRemembering)
                Response.AppendRefreshToken("token", 1);
            return Ok();//jwt token
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
            if (isRemembering)
                Response.AppendRefreshToken("token", 1);
            return Ok();//jwt token
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
            Request.TryGetRefreshToken();
            Response.AppendRefreshToken("token", 1);
            return Ok();//jwt token
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [HttpPost("[action]")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            Response.DeleteRefreshToken();
            return Ok();
        }
    }
}
