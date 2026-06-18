using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Internal.Constants;
using Application.Models.Internal.Messages;
using Application.Models.Internal.Options;
using Application.Models.OptionsAndHelpers;
using Application.Models.Output;
using Application.Services.Interfaces;
using Domain.Stores;
using Domain.Stores.MongoDB;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations
{
    public class PersonalChatService : IPersonalChatService
    {
        private readonly string _mediaPrefix;
        private readonly IPersonalChatStore _personalChatStore;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageCacheService _messageCacheService;
        private readonly IMessagesService _messagesService;
        private readonly IDeletedChatStore _deletedChatStore;
        public PersonalChatService(
            IPersonalChatStore personalChatStore, IMessagePublisher messagePublisher, 
            IMessageCacheService messageCacheService, IMessagesService messagesService, 
            IDeletedChatStore deletedChatStore, IOptions<ApplicationServicesOptions> options) 
        { 
            _personalChatStore = personalChatStore;
            _messagePublisher = messagePublisher;
            _messageCacheService = messageCacheService;
            _messagesService = messagesService;
            _deletedChatStore = deletedChatStore;
            _mediaPrefix = options.Value.MediaPrefix;
        }

        public async Task BlockUserAsync(Guid sourceUserId, Guid destinationUserId, CancellationToken cancellationToken)
        {
            await _personalChatStore.BlockUserAsync(sourceUserId, destinationUserId, cancellationToken);
        }

        public async Task DeleteChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            bool completed = false;
            try
            {
                await _deletedChatStore.SaveAsync(chatId, ChatTypeConverter.Convert(ChatType.Personal), cancellationToken);
                await _personalChatStore.DeleteChatAsync(chatId, userId, cancellationToken);
                completed = true;
            }
            catch (Exception ex) when (!completed)
            {
                await _deletedChatStore.DeleteAsync(chatId, ChatTypeConverter.Convert(ChatType.Personal), CancellationToken.None);
                throw ex;
            }
            finally
            {
                if (completed)
                await _messagePublisher.PublishAsync(
                        new ChatDeletedMessage()
                        {
                            ChatId = chatId,
                            ChatType = ChatType.Personal,
                            UserId = [userId, await _personalChatStore.GetUserIdByChatIdAsync(chatId, userId, cancellationToken)]
                        },
                        CancellationToken.None);
            }
        }

        public async Task DeleteFileFromMessageAsync(Guid userId, Guid chatId, Guid messageId, string mediaLink, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(mediaLink[(_mediaPrefix.Length + 1)..], out Guid mediaId))
                throw new DataValidationException("mediaLink");
            
            bool completed = false;
            try
            {
                await _messagesService.DeleteAttachmentAsync(userId, chatId, ChatType.Personal, messageId, mediaId, cancellationToken);
                completed = true;
            }
            finally
            {
                if (completed)
                {
                    await _messagePublisher.PublishAsync(
                    new FileDeletedMessage()
                    {
                        ChatId = chatId,
                        ChatType = ChatType.Personal,
                        UserId = [userId, await _personalChatStore.GetUserIdByChatIdAsync(chatId, userId, CancellationToken.None)],
                        File = $"{_mediaPrefix}{mediaId}",
                        MessageId = messageId
                    },
                    cancellationToken);

                    await _messageCacheService.InvalidateAsync(messageId, chatId, CancellationToken.None);
                }
            }
        }

        public async Task DeleteMessageAsync(Guid userId, Guid chatId, Guid messageId, CancellationToken cancellationToken)
        {
            bool completed = false;
            try
            {
                await _messagesService.DeleteMessageAsync(userId, ChatType.Personal, chatId, messageId, cancellationToken);
                completed = true;
            }
            finally
            {
                if (completed)
                {
                    await _messagePublisher.PublishAsync(
                    new MessageDeletedMessage()
                    {
                        ChatId = chatId,
                        ChatType = ChatType.Personal,
                        UserId = [userId, await _personalChatStore.GetUserIdByChatIdAsync(chatId, userId, CancellationToken.None)],
                        MessageId = messageId
                    },
                    cancellationToken);

                    await _messageCacheService.InvalidateAsync(messageId, chatId, CancellationToken.None);
                }
            }
        }

        public async Task EditMessageTextAsync(Guid userId, UpdatingMessage updatingMessage, CancellationToken cancellationToken)
        {
            bool completed = false;
            try
            {
                await _messagesService.UpdateMessageTextAsync(userId, ChatType.Personal, updatingMessage, cancellationToken);
                completed = true;
            }
            finally
            {
                if (completed)
                {
                    await _messagePublisher.PublishAsync(
                    new MessageUpdatedMessage()
                    {
                        ChatId = updatingMessage.ChatId,
                        ChatType = ChatType.Personal,
                        UserId = [userId, await _personalChatStore.GetUserIdByChatIdAsync(updatingMessage.ChatId, userId, CancellationToken.None)],
                        MessageId = updatingMessage.MessageId
                    },
                    cancellationToken);

                    await _messageCacheService.InvalidateAsync(updatingMessage.MessageId, updatingMessage.ChatId, CancellationToken.None);
                }
            }
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
                message = await _messagesService.GetMessageByIdAsync(userId, ChatType.Personal, chatId, messageId, cancellationToken);
                await _messageCacheService.SaveAsync(message, cancellationToken);
            }
            return message;
        }

        public async Task<Message[]> GetMessagesAsync(Guid userId, Guid chatId, MessagesSelectOptions options, CancellationToken cancellationToken)
        {
            return await _messagesService.GetMessagesAsync(userId, ChatType.Personal, chatId, options, cancellationToken);
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
                    ChatType = ChatType.Personal,                    
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
            bool completed = false;
            Guid[] ids = Array.Empty<Guid>();
            try
            {
                ids = await _messagesService.ResendMessagesAsync(userId, ChatType.Personal, resendMessagesModel, cancellationToken);
                completed = true;
            }
            finally
            {
                if (completed)
                    await _messagePublisher.PublishAsync(
                        new MessagesSentMessage()
                        {
                            ChatId = resendMessagesModel.ChatId,
                            ChatType = ChatType.Personal,
                            UserId = [userId, await _personalChatStore.GetUserIdByChatIdAsync(resendMessagesModel.ChatId, userId, CancellationToken.None)],
                            MessagesId = ids,
                        },
                        CancellationToken.None);
            }
            return ids;
        }

        public async Task<Guid> SendMessageAsync(SendingMessage sendingMessage, CancellationToken cancellationToken)
        {
            bool completed = false;
            Guid id = Guid.Empty;

            try
            {
                id = await _messagesService.SendUserMessageAsync(ChatType.Personal, sendingMessage, cancellationToken);
                completed = true;
            }
            finally
            {
                if (completed)
                    await _messagePublisher.PublishAsync(
                        new MessagesSentMessage()
                        {
                            ChatId = sendingMessage.ChatId,
                            ChatType = ChatType.Personal,
                            UserId = [sendingMessage.Author, await _personalChatStore.GetUserIdByChatIdAsync(sendingMessage.ChatId, sendingMessage.Author, CancellationToken.None)],
                            MessagesId = [id]
                        },
                        CancellationToken.None);
            }
            return id;
        }

        public async Task UnblockUserAsync(Guid sourceUserId, Guid destinationUserId, CancellationToken cancellationToken)
        {
            await _personalChatStore.UnblockUserAsync(sourceUserId, destinationUserId, cancellationToken);
        }
    }
}
