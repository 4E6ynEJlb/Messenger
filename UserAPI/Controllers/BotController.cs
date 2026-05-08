using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Models;

namespace UserAPI.Controllers
{
    [Authorize(Policy = Policies.USER_POLICY)]
    [Route("[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        [ProducesResponseType(typeof(Bot), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("{botId}")]
        public async Task<IActionResult> GetBotInfo(Guid botId)
        {
            return Ok();
        }

        [ProducesResponseType(typeof(Bot), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetBotByName(string botName)
        {
            return Ok();
        }

        [ProducesResponseType(typeof(Bot), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetBotByTag(string botTag)
        {
            return Ok();
        }

        [ProducesResponseType(typeof(Bot[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> ListPersonalBots()
        {
            return Ok();
        }

        [ProducesResponseType(typeof(BotToken), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetBotToken(Guid botId)
        {
            return Ok();
        }

        [ProducesResponseType(typeof(BotCommandInfo[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> ListBotCommands(Guid botId)
        {
            return Ok();
        }

        [ProducesResponseType(typeof(BotConnection[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> ListBotConnections(Guid botId)
        {
            return Ok();
        }

        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateBot([FromForm] CreateBotForm form)
        {
            return Ok();
        }

        [ProducesResponseType(typeof(uint), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddCommand(Guid botId, AddCommandModel addCommandModel)
        {
            return Ok();
        }

        [ProducesResponseType(typeof(uint), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddCommandArgument(Guid botId, AddArgumentModel addArgumentModel)
        {
            return Ok();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateBot(Guid botId, [FromForm] UpdateBotForm form)
        {
            return Ok();
        }

        [ProducesResponseType(typeof(BotToken), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPatch("[action]")]
        public async Task<IActionResult> RegenerateToken(Guid botId)
        {
            return Ok();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateCommand(Guid botId, UpdateCommandModel updateCommandModel)
        {
            return Ok();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateCommandArgument(Guid botId, UpdateArgumentModel updateArgumentModel)
        {
            return Ok();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteBot(Guid botId)
        {
            return Ok();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPatch("[action]")]
        public async Task<IActionResult> DeleteCommand(Guid botId, uint commandId)
        {
            return Ok();
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPatch("[action]")]
        public async Task<IActionResult> DeleteCommandArgument(Guid botId, uint commandId, uint argumentId)
        {
            return Ok();
        }
    }
}
