using Domain.Models.Types;

namespace Domain.Stores
{
    public interface IBotChatStore/////////impl
    {
        public Task<ChatInformation> GetChatShortInfoAsync(Guid chatId, Guid userId);
        public Task<char[]> GetBotCommandsPrefixesAsync(Guid chatId, Guid userId);
        public Task<object[]> GetBotCommandsListAsync(Guid chatId, Guid userId, uint pageNumber, uint pageSize);
        public Task<object[]> SearchBotCommandsAsync(Guid chatId, Guid userId, string prefix, uint pageNumber, uint pageSize);
        public Task<object[]> GetActiveButtonsListAsync(Guid chatId, Guid userId);        
        public Task<Message[]> GetMessagesAsync(Guid chatId, Guid gettingBy, uint messagesCount, DateTime sentBefore);
        public Task<Message> GetMessageAsync(Guid chatId, Guid messageId, Guid userId);
        public Task<Guid> GetBotIdByChatIdAsync(Guid chatId, Guid gettingBy);
        public Task<Guid> CreateChatAsync(Guid userId, Guid botId);
        public Task<Guid> SendMessageAsync(Guid chatId, Guid senderId, string text, MediaFile[]? attachments);
        public Task DisableBotAsync(Guid userId, Guid botId);
        public Task EnableBotAsync(Guid userId, Guid botId);
        public Task DeleteChatAsync(Guid chatId, Guid deletingBy);
    }
}
