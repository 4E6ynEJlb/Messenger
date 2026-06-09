using Domain.Models.Types;

namespace Domain.Stores
{
    public interface IBotControlStore
    {
        public Task<BotInfo> GetBotInfoAsync(Guid botId, CancellationToken cancellationToken);
        public Task<BotInfo> GetBotByNameAsync(string botName, CancellationToken cancellationToken);
        public Task<BotInfo> GetBotByTagAsync(string tag, CancellationToken cancellationToken);
        public Task<BotInfo[]> ListPersonalBotsAsync(Guid gettingBy, CancellationToken cancellationToken);
        public Task<BotTokenInfo> GetBotTokenAsync(Guid botId, Guid gettingBy, CancellationToken cancellationToken);
        public Task<BotCommandInfo[]> ListBotCommandsAsync(Guid botId, CancellationToken cancellationToken);
        public Task<BotConnectionLogRecord[]> ListBotConnectionsAsync(Guid botId, Guid gettingBy, CancellationToken cancellationToken);
        public Task<Guid> CreateBotAsync(string botName, string tag, string? botDescription, Guid owner, MediaFile? botAvatar, CancellationToken cancellationToken);
        public Task<uint> AddCommandAsync(Guid botId, Guid addingBy, char prefix, string command, string commandDescription, CancellationToken cancellationToken);
        public Task<uint> AddCommandArgumentAsync(Guid botId, Guid addingBy, uint commandId, string name, string type, CancellationToken cancellationToken);
        public Task UpdateBotAsync (Guid botId, Guid updatingBy, bool updateDescription, bool updateAvatar, string? newName, string? newTag, string? newDescription, MediaFile? newAvatar, CancellationToken cancellationToken);
        public Task<BotTokenInfo> RegenerateBotTokenAsync(Guid botId, Guid updatingBy, CancellationToken cancellationToken);
        public Task UpdateCommandAsync(Guid botId, Guid updatingBy, uint commandId, char? newPrefix, string? newCommand, string? newCommandDescription, CancellationToken cancellationToken);
        public Task UpdateCommandArgumentAsync(Guid botId, Guid updatingBy, uint commandId, uint argumentId, string? newName, string? newType, CancellationToken cancellationToken);
        public Task DeleteBotAsync(Guid botId, Guid deletingBy, CancellationToken cancellationToken);
        public Task DeleteCommandAsync(Guid botId, Guid deletingBy, uint commandId, CancellationToken cancellationToken);
        public Task DeleteCommandArgumentAsync(Guid botId, Guid deletingBy, uint commandId, uint argumentId, CancellationToken cancellationToken);
    }
}
