using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Database;

namespace Persistence.Repositories
{
    public class BotControlRepository : IBotControlStore
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public BotControlRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<uint> AddCommandArgumentAsync(Guid botId, Guid addingBy, uint commandId, string name, string type, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<uint> AddCommandAsync(Guid botId, Guid addingBy, char prefix, string command, string commandDescription, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateBotAsync(string botName, string tag, string? botDescription, Guid owner, MediaFile? botAvatar, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBotAsync(Guid botId, Guid deletingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCommandArgumentAsync(Guid botId, Guid deletingBy, uint commandId, uint argumentId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCommandAsync(Guid botId, Guid deletingBy, uint commandId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BotInfo> GetBotByNameAsync(string botName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BotInfo> GetBotByTagAsync(string tag, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BotInfo> GetBotInfoAsync(Guid botId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BotTokenInfo> GetBotTokenAsync(Guid botId, Guid gettingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BotCommandInfo[]> ListBotCommandsAsync(Guid botId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BotConnectionLogRecord[]> ListBotConnectionsAsync(Guid botId, Guid gettingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BotInfo[]> ListPersonalBotsAsync(Guid gettingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BotTokenInfo> RegenerateBotTokenAsync(Guid botId, Guid updatingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBotAsync(Guid botId, Guid updatingBy, bool updateDescription, bool updateAvatar, string? newName, string? newTag, string? newDescription, MediaFile? newAvatar, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCommandArgumentAsync(Guid botId, Guid updatingBy, uint commandId, uint argumentId, string? newName, string? newType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCommandAsync(Guid botId, Guid updatingBy, uint commandId, char? newPrefix, string? newCommand, string? newCommandDescription, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
