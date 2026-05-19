using Application.Models.Input;
using Application.Models.Output;

namespace Application.Services.Interfaces
{
    public interface IBotService
    {
        public Task<Bot> GetBotInfoAsync(Guid botId, CancellationToken cancellationToken);
        public Task<Bot> GetBotByNameAsync(string name, CancellationToken cancellationToken);
        public Task<Bot> GetBotByTagAsync(string tag, CancellationToken cancellationToken);
        public Task<Bot[]> ListPersonalBotsAsync(Guid userId, CancellationToken cancellationToken);
        public Task<BotToken> GetBotTokenAsync(Guid userId, Guid botId, CancellationToken cancellationToken);
        public Task<BotCommandInfo[]> ListBotCommandsAsync(Guid botId, CancellationToken cancellationToken);
        public Task<BotConnection[]> ListBotConnectionsAsync(Guid userId, Guid botId, CancellationToken cancellationToken);
        public Task<Guid> CreateBotAsync(Guid userId, CreateBotModel createBotModel, CancellationToken cancellationToken);
        public Task<uint> AddCommandAsync(Guid userId, Guid botId, AddCommandModel addCommandModel, CancellationToken cancellationToken);
        public Task<uint> AddCommandArgumentAsync(Guid userId, Guid botId, AddArgumentModel addArgumentModel, CancellationToken cancellationToken);
        public Task UpdateBotAsync(Guid userId, Guid botId, UpdateBotModel updateBotModel, CancellationToken cancellationToken);
        public Task<BotToken> RegenerateBotTokenAsync(Guid userId, Guid botId, CancellationToken cancellationToken);
        public Task UpdateCommandAsync(Guid userId, Guid botId, UpdateCommandModel updateCommandModel, CancellationToken cancellationToken);
        public Task UpdateCommandArgumentAsync(Guid userId, Guid botId, UpdateArgumentModel updateArgumentModel, CancellationToken cancellationToken);
        public Task DeleteBotAsync(Guid userId, Guid botId, CancellationToken cancellationToken);
        public Task DeleteCommandAsync(Guid userId, Guid botId, uint commandId, CancellationToken cancellationToken);
        public Task DeleteArgumentAsync(Guid userId, Guid botId, uint commandId, uint argumentId, CancellationToken cancellationToken);
    }
}
