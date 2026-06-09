using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Output;

namespace Application.Services.Interfaces
{
    public interface IPersonalChatService
    {
        public Task<ChatShortInfo> GetChatShortInfoAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
        public Task<Message[]> GetMessagesAsync(Guid userId, Guid chatId, MessagesSelectOptions options, CancellationToken cancellationToken);
        public Task<Message> GetMessageAsync(Guid userId, Guid chatId, Guid messageId, CancellationToken cancellationToken);
        public Task<Guid> GetUserIdByChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
        public Task<Guid> OpenChatWithUserAsync(Guid sourceUserId, Guid destinationUserId, CancellationToken cancellationToken);
        public Task<Guid> SendMessageAsync(SendingMessage sendingMessage,  CancellationToken cancellationToken);
        public Task<Guid[]> ResendMessagesAsync(Guid userId, ResendMessagesModel resendMessagesModel, CancellationToken cancellationToken);
        public Task HandleUserTypingEventAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
        public Task EditMessageTextAsync(Guid userId, UpdatingMessage updatingMessage, CancellationToken cancellationToken);
        public Task BlockUserAsync(Guid sourceUserId, Guid destinationUserId, CancellationToken cancellationToken);
        public Task UnblockUserAsync(Guid sourceUserId, Guid destinationUserId, CancellationToken cancellationToken);
        public Task DeleteMessageAsync(Guid userId, Guid chatId, Guid messageId, CancellationToken cancellationToken);
        public Task DeleteFileFromMessageAsync(Guid userId, Guid chatId, string mediaLink, CancellationToken cancellationToken);
        public Task DeleteChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
    }
}
