using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Output;

namespace Application.Services.Interfaces
{
    public interface IBotChatService
    {
        public Task<ChatShortInfo> GetChatShortInfoAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
        public Task<BotButton[]> GetActiveButtonsAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
        public Task<Message[]> GetMessagesAsync(Guid userId, Guid chatId, MessagesSelectOptions options, CancellationToken cancellationToken);
        public Task<Message> GetMessageAsync(Guid userId, Guid chatId, Guid messageId, CancellationToken cancellationToken);
        public Task<Guid> GetBotIdByChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
        public Task<Guid> OpenChatWithBotAsync(Guid userId, Guid botId, CancellationToken cancellationToken);
        public Task<Guid> SendMessageAsync(SendingMessage sendingMessage, CancellationToken cancellationToken);
        public Task<Guid[]> ResendMessagesAsync(Guid userId, ResendMessagesModel resendMessagesModel, CancellationToken cancellationToken);             
        public Task DisableBotAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
        public Task EnableBotAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
        public Task DeleteChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
    }
}
