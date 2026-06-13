using Domain.Models.Types;

namespace Domain.Stores
{
    public interface IBotChatStore
    {
        public Task<ChatInformation> GetChatShortInfoAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
        public Task<BotButtonInfo[]> GetActiveButtonsListAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);        
        public Task<Message[]> GetMessagesAsync(Guid chatId, Guid gettingBy, uint messagesCount, DateTime sentBefore, CancellationToken cancellationToken);
        public Task<Message> GetMessageAsync(Guid chatId, Guid messageId, Guid userId, CancellationToken cancellationToken);
        public Task<Guid> GetMessageIdByMediaAsync(Guid chatId, Guid mediaId, CancellationToken cancellationToken);
        public Task<Guid> GetBotIdByChatIdAsync(Guid chatId, Guid gettingBy, CancellationToken cancellationToken);
        public Task<bool> GetBotChatAbilityAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
        public Task<Guid> CreateChatAsync(Guid userId, Guid botId, CancellationToken cancellationToken);
        public Task<bool> CheckMessageSendingAbilityAsync(Guid chatId, Guid senderId, Guid? replyTo, CancellationToken cancellationToken);
        public Task DisableBotAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
        public Task EnableBotAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
        public Task DeleteChatAsync(Guid chatId, Guid deletingBy, CancellationToken cancellationToken);
    }
}
