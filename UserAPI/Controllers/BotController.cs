using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Output;
using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Storage;
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
        string mediaPrefix = "https://localhost:10102/Media";
        private readonly IBotControlStore _botControlStore;
        private readonly IObjectStorage _objectStorage;
        public BotController(IBotControlStore botControlStore, IObjectStorage objectStorage)
        {
            _botControlStore = botControlStore;
            _objectStorage = objectStorage;
        }
        [ProducesResponseType(typeof(Bot), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("{botId}")]
        public async Task<IActionResult> GetBotInfo(Guid botId, CancellationToken cancellationToken)
        {
            var b = await _botControlStore.GetBotInfoAsync(botId, cancellationToken);
            Bot bot = new Bot
            {
                BotId = b.BotId,
                Name = b.Name,
                Tag = b.Tag,
                Avatar = $"{mediaPrefix}/{b.Avatar}",
                Description = b.Description,
                IsEnabled = b.IsEnabled
            };
            return Ok();
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
            var b = await _botControlStore.GetBotByNameAsync(botName, cancellationToken);
            Bot bot = new Bot
            {
                BotId = b.BotId,
                Name = b.Name,
                Tag = b.Tag,
                Avatar = $"{mediaPrefix}/{b.Avatar}",
                Description = b.Description,
                IsEnabled = b.IsEnabled
            };
            return Ok(bot);
        }

        [ProducesResponseType(typeof(Bot), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetBotByTag(string botTag, CancellationToken cancellationToken)
        {
            var b = await _botControlStore.GetBotByTagAsync(botTag, cancellationToken);
            Bot bot = new Bot
            {
                BotId = b.BotId,
                Name = b.Name,
                Tag = b.Tag,
                Avatar = $"{mediaPrefix}/{b.Avatar}",
                Description = b.Description,
                IsEnabled = b.IsEnabled
            };
            return Ok(bot);
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
            var bots = await _botControlStore.ListPersonalBotsAsync(userId, cancellationToken);
            var result = bots.Select(b => new Bot
            {
                BotId = b.BotId,
                Name = b.Name,
                Tag = b.Tag,
                Avatar = $"{mediaPrefix}/{b.Avatar}",
                Description = b.Description,
                IsEnabled = b.IsEnabled
            }).ToArray();
            return Ok(result);
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
            var token = await _botControlStore.GetBotTokenAsync(botId, HttpContext.GetUserId(), cancellationToken);
            BotToken botToken = new BotToken
            {
                Token = Convert.ToBase64String(token.TokenHash),
                TokenVersion = token.TokenVersion
            };
            return Ok(botToken);
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
            var commands = await _botControlStore.ListBotCommandsAsync(botId, cancellationToken);
            var result = commands.Select(c => new Application.Models.Output.BotCommandInfo
            {
                Id = c.CommandId,
                Prefix = c.Prefix,
                Command = c.Command,
                Description = c.Description,
                Arguments = c.Arguments.Select(a => new CommandArgument
                {
                    Id = a.ArgumentId,
                    Name = a.Name,
                    Type = a.Type
                }).ToArray()
            }).ToArray();
            return Ok(result);
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
            var connections = await _botControlStore.ListBotConnectionsAsync(botId, HttpContext.GetUserId(), cancellationToken);
            var result = connections.Select(c => new BotConnection
            {
                IPAddress = c.IPAddress,
                ConnectedAt = c.ConnectedAt,
                TokenVersion = c.TokenVersion
            }).ToArray();
            return Ok(result);
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
            var MediaId = Guid.NewGuid();
            var avatar = form.BotAvatar == null ? null : new MediaFile
            {
                FileName = form.BotAvatar.FileName,
                ContentType = form.BotAvatar.ContentType,
                MediaId = MediaId,
            };
            var botId = await _botControlStore.CreateBotAsync(form.BotName, form.Tag, form.BotDescription, HttpContext.GetUserId(), avatar, cancellationToken);
            if (form.BotAvatar != null)
            {
                await _objectStorage.SaveAsync(form.BotAvatar.OpenReadStream(), MediaId, cancellationToken);
            }
            return Ok(botId);
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
            var commandId = await _botControlStore.AddCommandAsync(botId, HttpContext.GetUserId(), addCommandModel.Prefix, addCommandModel.Command, addCommandModel.Description, cancellationToken);
            return Ok(commandId);
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
            var argumentId = await _botControlStore.AddCommandArgumentAsync(botId, HttpContext.GetUserId(), addArgumentModel.CommandId, addArgumentModel.ArgumentName, addArgumentModel.ArgumentType, cancellationToken);
            return Ok(argumentId);
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
            Guid? newAvatarMediaId = form.UpdateAvatar ? (form.BotAvatar == null ? null : (Guid?)Guid.NewGuid()) : null;
            await _botControlStore.UpdateBotAsync(botId, HttpContext.GetUserId(), form.UpdateDescription, form.UpdateAvatar, form.BotName, form.Tag, form.UpdateDescription ? form.BotDescription : null, form.UpdateAvatar ? (form.BotAvatar == null ? null : new MediaFile
            {
                FileName = form.BotAvatar.FileName,
                ContentType = form.BotAvatar.ContentType,
                MediaId = newAvatarMediaId!.Value,
            }) : null, cancellationToken);
            if (form.UpdateAvatar && form.BotAvatar != null)
            {
                await _objectStorage.SaveAsync(form.BotAvatar.OpenReadStream(), newAvatarMediaId!.Value, cancellationToken);
            }
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
            var botToken = await _botControlStore.RegenerateBotTokenAsync(botId, HttpContext.GetUserId(), cancellationToken);
            BotToken result = new BotToken
            {
                Token = Convert.ToBase64String(botToken.TokenHash),
                TokenVersion = botToken.TokenVersion
            };
            return Ok(result);
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
            await _botControlStore.UpdateCommandAsync(botId, HttpContext.GetUserId(), updateCommandModel.Id, updateCommandModel.NewPrefix, updateCommandModel.NewCommand, updateCommandModel.NewDescription, cancellationToken);
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
            await _botControlStore.UpdateCommandArgumentAsync(botId, HttpContext.GetUserId(), updateArgumentModel.CommandId, updateArgumentModel.ArgumentId, updateArgumentModel.NewArgumentName, updateArgumentModel.NewArgumentType, cancellationToken);
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
            await _botControlStore.DeleteBotAsync(botId, HttpContext.GetUserId(), cancellationToken);
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
            await _botControlStore.DeleteCommandAsync(botId, HttpContext.GetUserId(), commandId, cancellationToken);
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
            await _botControlStore.DeleteCommandArgumentAsync(botId, HttpContext.GetUserId(), commandId, argumentId, cancellationToken);
            return Ok();
        }
    }
}
