using Application.Models.Internal.Constants;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Extensions;
using UserAPI.Models;

namespace UserAPI.Controllers
{
    [Authorize(Policy = Policies.USER_POLICY)]
    [Route("[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        public SecurityController(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        /// <summary>
        /// Only for not banned users
        /// </summary>
        /// <param name="chatType"></param>
        /// <param name="chatId"></param>
        /// <param name="messageId"></param>
        /// <param name="comment"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>404 if message not found</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportMessage(ChatType chatType, Guid chatId, Guid messageId, string? comment, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _securityService.ReportMessageAsync(userId, chatType, chatId, messageId, comment, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Only for not banned users
        /// </summary>
        /// <param name="reportedUserId"></param>
        /// <param name="comment"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportUser(Guid reportedUserId, string? comment, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _securityService.ReportUserAsync(userId, reportedUserId, comment, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Only for not banned users
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="comment"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportBot(Guid botId, string? comment, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _securityService.ReportBotAsync(userId, botId, comment, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Only for not banned users
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="comment"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportPublicChat(Guid chatId, string? comment, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _securityService.ReportPublicChatAsync(userId, chatId, comment, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// For banned or not banned users
        /// </summary>
        /// <param name="adminId"></param>
        /// <param name="comment"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Authorize(Policy = Policies.ANY_USER_POLICY)]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportAdministrator(int adminId, string? comment, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _securityService.ReportAdministratorAsync(userId, adminId, comment, cancellationToken);
            return Ok();
        }
    }
}
