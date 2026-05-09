using Application.Models.Internal.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Models;

namespace UserAPI.Controllers
{
    [Authorize(Policy = Policies.USER_POLICY)]
    [Route("[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportMessage(ChatType chatType, Guid chatId, Guid messageId, string? comment, CancellationToken cancellationToken)
        {
            return Ok();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportUser(Guid reportedUserId, string? comment, CancellationToken cancellationToken)
        {
            return Ok();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportBot(Guid botId, string? comment, CancellationToken cancellationToken)
        {
            return Ok();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportPublicChat(Guid chatId, string? comment, CancellationToken cancellationToken)
        {
            return Ok();
        }

        [Authorize(Policy = Policies.BANNED_USER_POLICY)]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> ReportAdministrator(int adminId, string? comment, CancellationToken cancellationToken)
        {
            return Ok();
        }
    }
}
