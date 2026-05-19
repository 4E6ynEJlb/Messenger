using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Internal.Messages;
using Application.Models.Internal.Options;
using Application.Models.OptionsAndHelpers;
using Application.Models.Output;
using Application.Services.Interfaces;
using Domain.Stores;
using Infrastructure.Storage;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations
{
    public class PersonalChatService : IPersonalChatService
    {
        private readonly string _mediaPrefix;
        private readonly IPersonalChatStore _personalChatStore;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IObjectStorage _objectStorage;
        private readonly IMessageCacheService _messageCacheService;
        public PersonalChatService(
            IPersonalChatStore personalChatStore, IMessagePublisher messagePublisher, 
            IObjectStorage objectStorage, IMessageCacheService messageCacheService, IOptions<ApplicationServicesOptions> options) 
        { 
            _personalChatStore = personalChatStore;
            _messagePublisher = messagePublisher;
            _objectStorage = objectStorage;
            _messageCacheService = messageCacheService;
            _mediaPrefix = options.Value.MediaPrefix;
        }

        public async Task BlockUserAsync(Guid sourceUserId, Guid destinationUserId, CancellationToken cancellationToken)
        {
            await _personalChatStore.BlockUserAsync(sourceUserId, destinationUserId, cancellationToken);
        }

        public async Task DeleteChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            await _personalChatStore.DeleteChatAsync(chatId, userId, cancellationToken);
            await _messagePublisher.PublishAsync(
                new ChatDeletedMessage() 
                { 
                    ChatId = chatId,
                    ChatType = Models.Internal.Constants.ChatType.Personal,
                    UserId = [userId, await _personalChatStore.GetUserIdByChatIdAsync(chatId, userId, cancellationToken)]
                }, 
                cancellationToken);
        }

        public async Task DeleteFileFromMessageAsync(Guid userId, Guid chatId, Guid mediaId, CancellationToken cancellationToken)
        {
            Guid messageId = await _personalChatStore.GetMessageIdByMediaAsync(chatId, mediaId, cancellationToken);
            await _personalChatStore.DeleteFileFromMessageAsync(chatId, mediaId, userId, cancellationToken);
            await _messagePublisher.PublishAsync(
                new FileDeletedMessage() 
                { 
                    ChatId = chatId,
                    ChatType = Models.Internal.Constants.ChatType.Personal,
                    UserId = [userId, await _personalChatStore.GetUserIdByChatIdAsync(chatId, userId, cancellationToken)],
                    File = $"{_mediaPrefix}{mediaId}",
                    MessageId = messageId
                }, 
                cancellationToken);
            await _messageCacheService.InvalidateAsync(messageId, chatId, cancellationToken);
        }

        public async Task DeleteMessageAsync(Guid userId, Guid chatId, Guid messageId, CancellationToken cancellationToken)
        {
            await _personalChatStore.DeleteMessageAsync(chatId, messageId, userId, cancellationToken);
            await _messagePublisher.PublishAsync(
                new MessageDeletedMessage() 
                { 
                    ChatId = chatId,
                    ChatType = Models.Internal.Constants.ChatType.Personal,
                    UserId = [userId, await _personalChatStore.GetUserIdByChatIdAsync(chatId, userId, cancellationToken)],
                    MessageId = messageId
                }, 
                cancellationToken);
            await _messageCacheService.InvalidateAsync(messageId, chatId, cancellationToken);
        }

        public async Task EditMessageTextAsync(Guid userId, UpdatingMessage updatingMessage, CancellationToken cancellationToken)
        {
            await _personalChatStore.UpdateMessageTextAsync(updatingMessage.ChatId, updatingMessage.MessageId, userId, updatingMessage.MessageText, cancellationToken);
            await _messagePublisher.PublishAsync(
                new MessageUpdatedMessage() 
                { 
                    ChatId = updatingMessage.ChatId,
                    ChatType = Models.Internal.Constants.ChatType.Personal,
                    UserId = [userId, await _personalChatStore.GetUserIdByChatIdAsync(updatingMessage.ChatId, userId, cancellationToken)],
                    MessageId = updatingMessage.MessageId
                }, 
                cancellationToken);
            await _messageCacheService.InvalidateAsync(updatingMessage.MessageId, updatingMessage.ChatId, cancellationToken);
        }

        public async Task<ChatShortInfo> GetChatShortInfoAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            Domain.Models.Types.ChatInformation chatShortInfo = await _personalChatStore.GetChatShortInfoAsync(chatId, userId, cancellationToken);
            return new ChatShortInfo(chatShortInfo, _mediaPrefix);
        }

        public async Task<Message> GetMessageAsync(Guid userId, Guid chatId, Guid messageId, CancellationToken cancellationToken)
        {
            Message? message = await _messageCacheService.GetAsync(messageId, chatId, cancellationToken);
            if (message is null)
            {
                Domain.Models.Types.Message messageData = await _personalChatStore.GetMessageAsync(chatId, messageId, userId, cancellationToken);
                message = new Message(messageData, _mediaPrefix, chatId);
                await _messageCacheService.SaveAsync(message, cancellationToken);
            }
            return message;
        }

        public async Task<Message[]> GetMessagesAsync(Guid userId, Guid chatId, MessagesSelectOptions options, CancellationToken cancellationToken)
        {
            Domain.Models.Types.Message[] messages = await _personalChatStore.GetMessagesAsync(chatId, userId, options.MessagesCount, options.SentBefore ?? DateTime.UtcNow, cancellationToken);
            return messages.Select(m => new Message(m, _mediaPrefix, chatId)).ToArray();
        }

        public async Task<Guid> GetUserIdByChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            return await _personalChatStore.GetUserIdByChatIdAsync(chatId, userId, cancellationToken);
        }

        public async Task HandleUserTypingEventAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            await _messagePublisher.PublishAsync(
                new UserIsTypingMessage() 
                { 
                    ChatId = chatId,
                    ChatType = Models.Internal.Constants.ChatType.Personal,                    
                    TypingUserId = userId,
                    DestinationUserId = [await _personalChatStore.GetUserIdByChatIdAsync(chatId, userId, cancellationToken)]
                }, 
                cancellationToken);
        }

        public async Task<Guid> OpenChatWithUserAsync(Guid sourceUserId, Guid destinationUserId, CancellationToken cancellationToken)
        {
            return await _personalChatStore.CreateChatAsync(sourceUserId, destinationUserId, cancellationToken);
        }

        public async Task<Guid[]> ResendMessagesAsync(Guid userId, ResendMessagesModel resendMessagesModel, CancellationToken cancellationToken)
        {
            Guid[] ids = await _personalChatStore.ResendMessagesAsync(resendMessagesModel.ChatId, userId, ChatTypeConverter.Convert(resendMessagesModel.SourceChatType), resendMessagesModel.SourceChatId, resendMessagesModel.Messages, cancellationToken);
            await _messagePublisher.PublishAsync(
                new MessagesSentMessage() 
                { 
                    ChatId = resendMessagesModel.ChatId,
                    ChatType = Models.Internal.Constants.ChatType.Personal,
                    UserId = [userId, await _personalChatStore.GetUserIdByChatIdAsync(resendMessagesModel.ChatId, userId, cancellationToken)],
                    MessagesId = ids,
                }, 
                cancellationToken);
            return ids;
        }

        public async Task<Guid> SendMessageAsync(Guid userId, SendingMessage sendingMessage, CancellationToken cancellationToken)
        {
            Domain.Models.Types.MediaFile[]? attachments = null;
            if (sendingMessage.Attachments != null && sendingMessage.Attachments.Length > 0)
            {
                var list = new List<Domain.Models.Types.MediaFile>(sendingMessage.Attachments.Length);
                foreach (var a in sendingMessage.Attachments)
                {
                    var mediaId = Guid.NewGuid();
                    await _objectStorage.SaveAsync(a.Content, mediaId, cancellationToken);
                    list.Add(a.ToMediaFile(mediaId));
                }
                attachments = list.ToArray();
            }

            Guid id = await _personalChatStore.SendMessageAsync(
                sendingMessage.ChatId, userId,
                sendingMessage.ReplyTo, sendingMessage.MessageText,
                attachments, cancellationToken);

            await _messagePublisher.PublishAsync(
                new MessagesSentMessage()
                {
                    ChatId = sendingMessage.ChatId,
                    ChatType = Models.Internal.Constants.ChatType.Personal,
                    UserId = [userId, await _personalChatStore.GetUserIdByChatIdAsync(sendingMessage.ChatId, userId, cancellationToken)],
                    MessagesId = [id]
                },
                cancellationToken);

            return id;
        }

        public async Task UnblockUserAsync(Guid sourceUserId, Guid destinationUserId, CancellationToken cancellationToken)
        {
            await _personalChatStore.UnblockUserAsync(sourceUserId, destinationUserId, cancellationToken);
        }
    }
}
