using Domain.Models.Types;

namespace Domain.Stores
{
    public interface IBotChatStore
    {
        public Task<ChatInformation> GetChatShortInfoAsync(Guid chatId, Guid userId);
        public Task<BotButtonInfo[]> GetActiveButtonsListAsync(Guid chatId, Guid userId);        
        public Task<Message[]> GetMessagesAsync(Guid chatId, Guid gettingBy, uint messagesCount, DateTime sentBefore);
        public Task<Message> GetMessageAsync(Guid chatId, Guid messageId, Guid userId);
        public Task<Guid> GetBotIdByChatIdAsync(Guid chatId, Guid gettingBy);
        public Task<bool> GetBotChatAbilityAsync(Guid chatId, Guid userId);
        public Task<Guid> CreateChatAsync(Guid userId, Guid botId);
        public Task<Guid> SendMessageAsync(Guid chatId, Guid senderId, Guid? replyTo, string text, MediaFile[]? attachments);
        public Task<Guid[]> ResendMessagesAsync(Guid chatId, Guid senderId, EnChatType sourceChatType, Guid sourceChatId, Guid[] messages);
        public Task DisableBotAsync(Guid userId, Guid chatId);
        public Task EnableBotAsync(Guid userId, Guid chatId);
        public Task DeleteChatAsync(Guid chatId, Guid deletingBy);
    }
}
