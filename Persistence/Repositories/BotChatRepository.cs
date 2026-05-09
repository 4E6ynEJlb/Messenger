using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Database;

namespace Persistence.Repositories
{
    public class BotChatRepository : IBotChatStore
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public BotChatRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<Guid> CreateChatAsync(Guid userId, Guid botId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteChatAsync(Guid chatId, Guid deletingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DisableBotAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task EnableBotAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BotButtonInfo[]> GetActiveButtonsListAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetBotChatAbilityAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> GetBotIdByChatIdAsync(Guid chatId, Guid gettingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ChatInformation> GetChatShortInfoAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Message> GetMessageAsync(Guid chatId, Guid messageId, Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Message[]> GetMessagesAsync(Guid chatId, Guid gettingBy, uint messagesCount, DateTime sentBefore, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid[]> ResendMessagesAsync(Guid chatId, Guid senderId, EnChatType sourceChatType, Guid sourceChatId, Guid[] messages, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> SendMessageAsync(Guid chatId, Guid senderId, Guid? replyTo, string text, MediaFile[]? attachments, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
