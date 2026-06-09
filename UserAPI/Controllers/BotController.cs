using Application.Models.Input;
using Application.Models.Output;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Extensions;
using UserAPI.Models;

namespace UserAPI.Controllers
{
    /// <summary>
    /// Only for not banned users
    /// </summary>
    [Authorize(Policy = Policies.USER_POLICY)]
    [Route("[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotService _botService;
        public BotController(IBotService botService)
        {
            _botService = botService;
        }

        [ProducesResponseType(typeof(Bot), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("{botId}")]
        public async Task<IActionResult> GetBotInfo(Guid botId, CancellationToken cancellationToken)
        {            
            return Ok(await _botService.GetBotInfoAsync(botId, cancellationToken));
        }

        /// <summary>
        /// Searches bot by its name or its part, length >= 3 chars
        /// </summary>
        /// <param name="botName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Bot), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetBotByName(string botName, CancellationToken cancellationToken)
        {            
            return Ok(await _botService.GetBotByNameAsync(botName, cancellationToken));
        }

        [ProducesResponseType(typeof(Bot), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetBotByTag(string botTag, CancellationToken cancellationToken)
        {
            return Ok(await _botService.GetBotByTagAsync(botTag, cancellationToken));
        }

        /// <summary>
        /// Gets list of bots created by current user
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Bot[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> ListPersonalBots(CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _botService.ListPersonalBotsAsync(userId, cancellationToken));
        }

        /// <summary>
        /// Gets bot token only by its owner
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(BotToken), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetBotToken(Guid botId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _botService.GetBotTokenAsync(botId, userId, cancellationToken));
        }

        /// <summary>
        /// Call when user opens bot chat page to get list of commands 
        /// with arguments for this bot or when owner opens bot commands configuration
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Application.Models.Output.BotCommandInfo[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> ListBotCommands(Guid botId, CancellationToken cancellationToken)
        {
            return Ok(await _botService.ListBotCommandsAsync(botId, cancellationToken));
        }

        /// <summary>
        /// Gets connections to bot with info about IP address. 
        /// Available only for bot owner
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(BotConnection[]), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> ListBotConnections(Guid botId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _botService.ListBotConnectionsAsync(userId, botId, cancellationToken));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Created bot id</returns>
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateBot([FromForm] CreateBotForm form, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _botService.CreateBotAsync(userId, form.ToCreateBotModel(), cancellationToken));
        }

        /// <summary>
        /// Creates command for bot. Available only for bot owner.
        /// Commands should be shown in bot chat as tips for users
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="addCommandModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Created command id</returns>
        [ProducesResponseType(typeof(uint), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddCommand(Guid botId, AddCommandModel addCommandModel, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _botService.AddCommandAsync(userId, botId, addCommandModel, cancellationToken));
        }

        /// <summary>
        /// Creates command argument for command. Available only for bot owner. 
        /// Arguments should be shown in bot chat as tips for users after they enter command with prefix
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="addArgumentModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Created argument id</returns>
        [ProducesResponseType(typeof(uint), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddCommandArgument(Guid botId, AddArgumentModel addArgumentModel, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _botService.AddCommandArgumentAsync(userId, botId, addArgumentModel, cancellationToken));
        }

        /// <summary>
        /// Available for bot owner
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="form"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateBot(Guid botId, [FromForm] UpdateBotForm form, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _botService.UpdateBotAsync(userId, botId, form.ToUpdateBotModel(), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Generates new bot token and invalidates old one. Available only for bot owner
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>New token</returns>
        [ProducesResponseType(typeof(BotToken), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPatch("[action]")]
        public async Task<IActionResult> RegenerateToken(Guid botId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _botService.RegenerateBotTokenAsync(userId, botId, cancellationToken));
        }

        /// <summary>
        /// Available only for bot owner
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="updateCommandModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateCommand(Guid botId, UpdateCommandModel updateCommandModel, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _botService.UpdateCommandAsync(userId, botId, updateCommandModel, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Available only for bot owner
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="updateArgumentModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateCommandArgument(Guid botId, UpdateArgumentModel updateArgumentModel, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _botService.UpdateCommandArgumentAsync(userId, botId, updateArgumentModel, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Available only for bot owner
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteBot(Guid botId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId(); 
            await _botService.DeleteBotAsync(userId, botId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Available only for bot owner. When command is deleted, 
        /// all commands with greater id should decrease their id by 1
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="commandId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPatch("[action]")]
        public async Task<IActionResult> DeleteCommand(Guid botId, uint commandId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _botService.DeleteCommandAsync(userId, botId, commandId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Available only for bot owner. When argument is deleted, 
        /// all arguments with greater id for this command should decrease their id by 1
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="commandId"></param>
        /// <param name="argumentId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpPatch("[action]")]
        public async Task<IActionResult> DeleteCommandArgument(Guid botId, uint commandId, uint argumentId, CancellationToken cancellationToken)
        {
            Guid userId = HttpContext.GetUserId();
            await _botService.DeleteArgumentAsync(userId, botId, commandId, argumentId, cancellationToken);
            return Ok();
        }
    }
}
