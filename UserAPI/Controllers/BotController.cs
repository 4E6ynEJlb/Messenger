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
                TokenHash = token.TokenHash,
                TokenVersion = token.TokenVersion
            };
            return Ok(botToken);
        }

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
                TokenHash = botToken.TokenHash,
                TokenVersion = botToken.TokenVersion
            };
            return Ok(result);
        }

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
