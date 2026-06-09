using Application.Models.Input;
using Application.Models.Internal.Options;
using Application.Models.Output;
using Application.Services.Interfaces;
using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Storage;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations
{
    public class BotService : IBotService
    {
        private readonly string _mediaPrefix;
        private readonly IBotUserCacheService _cache;
        private readonly IBotControlStore _botStore;
        private readonly IObjectStorage _objectStorage;
        public BotService(IBotControlStore botStore,
            IObjectStorage objectStorage, IBotUserCacheService cache,
            IOptions<ApplicationServicesOptions> options)
        {
            _botStore = botStore;
            _cache = cache;
            _objectStorage = objectStorage;
            _mediaPrefix = options.Value.MediaPrefix;
        }
        public async Task<uint> AddCommandArgumentAsync(Guid userId, Guid botId, AddArgumentModel addArgumentModel, CancellationToken cancellationToken)
        {
            return await _botStore.AddCommandArgumentAsync(
                botId, userId, addArgumentModel.CommandId, 
                addArgumentModel.ArgumentName, addArgumentModel.ArgumentType, 
                cancellationToken);
        }

        public async Task<uint> AddCommandAsync(Guid userId, Guid botId, AddCommandModel addCommandModel, CancellationToken cancellationToken)
        {
            return await _botStore.AddCommandAsync(
                botId, userId, addCommandModel.Prefix, 
                addCommandModel.Command, addCommandModel.Description, 
                cancellationToken);
        }

        public async Task<Guid> CreateBotAsync(Guid userId, CreateBotModel createBotModel, CancellationToken cancellationToken)
        {
            Guid mediaId = Guid.NewGuid();
            MediaFile? avatar = createBotModel.BotAvatar is null ? null : createBotModel.BotAvatar.ToMediaFile(mediaId);
            Guid botId = await _botStore.CreateBotAsync(
                createBotModel.BotName, createBotModel.Tag, 
                createBotModel.BotDescription, userId, avatar, cancellationToken);
            if (createBotModel.BotAvatar is not null)
            {
                await _objectStorage.SaveAsync(createBotModel.BotAvatar.Content, mediaId, cancellationToken);
            }
            return botId;
        }

        public async Task DeleteArgumentAsync(Guid userId, Guid botId, uint commandId, uint argumentId, CancellationToken cancellationToken)
        {
            await _botStore.DeleteCommandArgumentAsync(botId, userId, commandId, argumentId, cancellationToken);
        }

        public async Task DeleteBotAsync(Guid userId, Guid botId, CancellationToken cancellationToken)
        {
            await _botStore.DeleteBotAsync(botId, userId, cancellationToken);
            await _cache.InvalidateAsync(botId, cancellationToken);
        }

        public async Task DeleteCommandAsync(Guid userId, Guid botId, uint commandId, CancellationToken cancellationToken)
        {
            await _botStore.DeleteCommandAsync(botId, userId, commandId, cancellationToken);
        }

        public async Task<Bot> GetBotByNameAsync(string name, CancellationToken cancellationToken)
        {
            BotInfo botInfo = await _botStore.GetBotByNameAsync(name, cancellationToken);
            Bot bot = new Bot(botInfo, _mediaPrefix);
            await _cache.SaveBotAsync(bot, cancellationToken);
            return bot;
        }

        public async Task<Bot> GetBotByTagAsync(string tag, CancellationToken cancellationToken)
        {
            BotInfo botInfo = await _botStore.GetBotByTagAsync(tag, cancellationToken);
            Bot bot = new Bot(botInfo, _mediaPrefix);
            await _cache.SaveBotAsync(bot, cancellationToken);
            return bot;
        }

        public async Task<Bot> GetBotInfoAsync(Guid botId, CancellationToken cancellationToken)
        {
            Bot? bot = await _cache.GetBotAsync(botId, cancellationToken);
            if (bot is null)
            {
                BotInfo botInfo = await _botStore.GetBotInfoAsync(botId, cancellationToken);
                bot = new Bot(botInfo, _mediaPrefix);
                await _cache.SaveBotAsync(bot, cancellationToken);
            }
            return bot;
        }

        public async Task<BotToken> GetBotTokenAsync(Guid userId, Guid botId, CancellationToken cancellationToken)
        {
            BotTokenInfo botTokenInfo = await _botStore.GetBotTokenAsync(botId, userId, cancellationToken);
            BotToken botToken = new BotToken(botTokenInfo);
            return botToken;
        }

        public async Task<Models.Output.BotCommandInfo[]> ListBotCommandsAsync(Guid botId, CancellationToken cancellationToken)
        {
            Domain.Models.Types.BotCommandInfo[] commands = 
                await _botStore.ListBotCommandsAsync(botId, cancellationToken);
            Models.Output.BotCommandInfo[] result = 
                commands.Select(c => new Models.Output.BotCommandInfo(c)).ToArray();
            return result;
        }

        public async Task<BotConnection[]> ListBotConnectionsAsync(Guid userId, Guid botId, CancellationToken cancellationToken)
        {
            BotConnectionLogRecord[] connections = await _botStore.ListBotConnectionsAsync(botId, userId, cancellationToken);
            BotConnection[] result = connections.Select(c => new BotConnection(c)).ToArray();
            return result;
        }

        public async Task<Bot[]> ListPersonalBotsAsync(Guid userId, CancellationToken cancellationToken)
        {
            BotInfo[] bots = await _botStore.ListPersonalBotsAsync(userId, cancellationToken);
            Bot[] result = bots.Select(b => new Bot(b, _mediaPrefix)).ToArray();
            return result;
        }

        public async Task<BotToken> RegenerateBotTokenAsync(Guid userId, Guid botId, CancellationToken cancellationToken)
        {
            BotTokenInfo botTokenInfo = await _botStore.RegenerateBotTokenAsync(botId, userId, cancellationToken);
            BotToken botToken = new BotToken(botTokenInfo);
            return botToken;
        }

        public async Task UpdateBotAsync(Guid userId, Guid botId, UpdateBotModel updateBotModel, CancellationToken cancellationToken)
        {
            Guid? newAvatarMediaId = 
                updateBotModel.UpdateAvatar ? 
                    (updateBotModel.BotAvatar is null ? 
                    null : 
                    Guid.NewGuid()) : 
                null;

            await _botStore.UpdateBotAsync(botId, userId, 
                updateBotModel.UpdateDescription, updateBotModel.UpdateAvatar, 
                updateBotModel.BotName, updateBotModel.Tag, 
                updateBotModel.UpdateDescription ? updateBotModel.BotDescription : null, 
                updateBotModel.UpdateAvatar ? 
                    (updateBotModel.BotAvatar is null || !newAvatarMediaId.HasValue ? null : 
                updateBotModel.BotAvatar.ToMediaFile(newAvatarMediaId.Value)) : null, cancellationToken);
            
            if (updateBotModel.UpdateAvatar && updateBotModel.BotAvatar is not null && newAvatarMediaId.HasValue)
            {
                await _objectStorage.SaveAsync(updateBotModel.BotAvatar.Content, newAvatarMediaId.Value, cancellationToken);
            }

            await _cache.InvalidateAsync(botId, cancellationToken);
        }

        public async Task UpdateCommandArgumentAsync(Guid userId, Guid botId, UpdateArgumentModel updateArgumentModel, CancellationToken cancellationToken)
        {
            await _botStore.UpdateCommandArgumentAsync(botId, userId, updateArgumentModel.CommandId, 
                updateArgumentModel.ArgumentId, updateArgumentModel.NewArgumentName, 
                updateArgumentModel.NewArgumentType, cancellationToken);
        }

        public async Task UpdateCommandAsync(Guid userId, Guid botId, UpdateCommandModel updateCommandModel, CancellationToken cancellationToken)
        {
            await _botStore.UpdateCommandAsync(botId, userId, updateCommandModel.Id, 
                updateCommandModel.NewPrefix, updateCommandModel.NewCommand, 
                updateCommandModel.NewDescription, cancellationToken);
        }
    }
}
