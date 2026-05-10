using Application.Models.Internal.Constants;
using Domain.Models.Types;
using Domain.Stores;
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
        private readonly ISecurityStore _securityStore;
        public SecurityController(ISecurityStore securityStore)
        {
            _securityStore = securityStore;
        }
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportMessage(ChatType chatType, Guid chatId, Guid messageId, string? comment, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();
            await _securityStore.ReportMessageAsync(userId, chatType switch { ChatType.Personal => EnChatType.Personal, ChatType.Group => EnChatType.Public, ChatType.Bot => EnChatType.Bot, _ => throw new ArgumentOutOfRangeException() }, chatId, messageId, comment, cancellationToken);
            return Ok();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportUser(Guid reportedUserId, string? comment, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();
            await _securityStore.ReportUserAsync(userId, reportedUserId, comment, cancellationToken);
            return Ok();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportBot(Guid botId, string? comment, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();
            await _securityStore.ReportBotAsync(userId, botId, comment, cancellationToken);
            return Ok();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportPublicChat(Guid chatId, string? comment, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();
            await _securityStore.ReportPublicChatAsync(userId, chatId, comment, cancellationToken);
            return Ok();
        }

        [Authorize(Policy = Policies.ANY_USER_POLICY)]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportAdministrator(int adminId, string? comment, CancellationToken cancellationToken)
        {
            var userId = HttpContext.GetUserId();
            await _securityStore.ReportAdministratorAsync(userId, adminId, comment, cancellationToken);
            return Ok();
        }
    }
}
