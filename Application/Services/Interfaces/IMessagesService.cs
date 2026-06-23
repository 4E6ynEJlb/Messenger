using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Internal.Constants;
using Application.Models.Output;

namespace Application.Services.Interfaces
{
    public interface IMessagesService
    {
        public Task<Message> GetMessageByIdAsync(Guid userId, ChatType chatType, Guid chatId, Guid messageId, CancellationToken cancellationToken);
        public Task<Message[]> GetMessagesAsync(Guid userId, ChatType chatType, Guid chatId, MessagesSelectOptions options, CancellationToken cancellationToken);
        public Task<Guid> SendUserMessageAsync(ChatType chatType, SendingMessage sendingMessage, CancellationToken cancellationToken);
        public Task<Guid[]> ResendMessagesAsync(Guid userId, ChatType chatType, ResendMessagesModel resendMessagesModel, CancellationToken cancellationToken);
        public Task UpdateMessageTextAsync(Guid userId, ChatType chatType, UpdatingMessage updatingMessage, CancellationToken cancellationToken);
        public Task DeleteAttachmentAsync(Guid userId, Guid chatId, ChatType chatType, Guid messageId, Guid mediaId, CancellationToken cancellationToken);
        public Task DeleteMessageAsync(Guid userId, ChatType chatType, Guid chatId, Guid messageId, CancellationToken cancellationToken);
    }
}
