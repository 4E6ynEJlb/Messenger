using Domain.Models.Types;

namespace Domain.Stores
{
    public interface IBotControlStore
    {
        public Task<BotInfo> GetBotInfoAsync(Guid botId);
        public Task<BotInfo> GetBotByNameAsync(string botName);
        public Task<BotInfo> GetBotByTagAsync(string tag);
        public Task<BotInfo[]> ListPersonalBotsAsync(Guid gettingBy);
        public Task<BotTokenInfo> GetBotTokenAsync(Guid botId, Guid gettingBy);
        public Task<BotCommandInfo[]> ListBotCommandsAsync(Guid botId);
        public Task<BotConnectionLogRecord[]> ListBotConnectionsAsync(Guid botId, Guid gettingBy);
        public Task<Guid> CreateBotAsync(string botName, string tag, string? botDescription, Guid owner, MediaFile? botAvatar);
        public Task<uint> AddCommandAsync(Guid botId, Guid addingBy, char prefix, string command, string commandDescription);
        public Task<uint> AddCommandArgumentAsync(Guid botId, Guid addingBy, uint commandId, string name, string type);
        public Task UpdateBotAsync (Guid botId, Guid updatingBy, bool updateDescription, bool updateAvatar, string? newName, string? newTag, string? newDescription, MediaFile? newAvatar);
        public Task<BotTokenInfo> RegenerateBotTokenAsync(Guid botId, Guid updatingBy);
        public Task UpdateCommandAsync(Guid botId, Guid updatingBy, uint commandId, char? newPrefix, string? newCommand, string? newCommandDescription);
        public Task UpdateCommandArgumentAsync(Guid botId, Guid updatingBy, uint commandId, uint argumentId, string? newName, string? newType);
        public Task DeleteBotAsync(Guid botId, Guid deletingBy);
        public Task DeleteCommandAsync(Guid botId, Guid deletingBy, uint commandId);
        public Task DeleteCommandArgumentAsync(Guid botId, Guid deletingBy, uint commandId, uint argumentId);
    }
}
