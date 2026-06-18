using Application.Models.Helpers;
using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Internal.Constants;
using Application.Models.Internal.Messages;
using Application.Models.Internal.Options;
using Application.Models.OptionsAndHelpers;
using Application.Models.Output;
using Application.Services.Interfaces;
using Domain.Models.Types;
using Domain.Stores;
using Domain.Stores.MongoDB;
using Infrastructure.Storage;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations
{
    public class PublicChatService : IPublicChatService
    {
        private readonly string _mediaPrefix;
        private readonly IPublicChatStore _publicChatStore;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMessageCacheService _messageCacheService;
        private readonly IMessagesService _messagesService;
        private readonly IDeletedChatStore _deletedChatStore;
        private readonly IObjectStorage _objectStorage;
        public PublicChatService(
            IPublicChatStore publicChatStore, IMessagePublisher messagePublisher,
            IMessageCacheService messageCacheService, IMessagesService messagesService,
            IDeletedChatStore deletedChatStore, IObjectStorage objectStorage,
            IOptions<ApplicationServicesOptions> options)
        {
            _publicChatStore = publicChatStore;
            _messagePublisher = messagePublisher;
            _messageCacheService = messageCacheService;
            _messagesService = messagesService;
            _deletedChatStore = deletedChatStore;
            _objectStorage = objectStorage;
            _mediaPrefix = options.Value.MediaPrefix;
        }

        public async Task<PublicChatAuditRecord[]> AuditChatAsync(Guid userId, Guid chatId, PageOptions pageOptions, CancellationToken cancellationToken)
        {
            AuditLogRecord[] auditLog = await _publicChatStore.AuditChatAsync(chatId, userId, pageOptions.Page, pageOptions.PageSize, cancellationToken);
            return auditLog.Select(r => new PublicChatAuditRecord(r)).ToArray();
        }

        public async Task BanUserAsync(Guid userId, Guid chatId, Guid bannedUserId, CancellationToken cancellationToken)
        {
            await _publicChatStore.BanUserAsync(chatId, bannedUserId, userId, cancellationToken);
        }

        public async Task<Guid> CreateChatAsync(Guid creatorId, string name, bool isSearchable, FileUpload? avatar, PublicChatMemberRole defaultRole, CancellationToken cancellationToken)
        {
            Guid mediaId = Guid.NewGuid();
            bool completed = false;
            Guid chatId = Guid.Empty;
            try
            {
                chatId = await _publicChatStore.CreateNewChatAsync(name, creatorId, isSearchable, avatar?.ToMediaFile(mediaId), ChatRoleConverter.Convert(defaultRole), cancellationToken);
                completed = true;
            }
            finally
            {
                if (completed && avatar is not null)
                {
                    await _objectStorage.SaveAsync(avatar.Content, mediaId, CancellationToken.None);
                }
            }
            return chatId;
        }

        public async Task DeleteChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            Guid[] users = (await _publicChatStore.GetChatFullInfoAsync(chatId, userId, cancellationToken))
                    .Members.Select(m => m.UserId).ToArray();
            bool completed = false;
            try
            {
                await _deletedChatStore.SaveAsync(chatId, ChatTypeConverter.Convert(ChatType.Group), cancellationToken);
                await _publicChatStore.DeleteChatAsync(chatId, userId, cancellationToken);
                completed = true;
            }
            catch (Exception ex) when (!completed)
            {
                await _deletedChatStore.DeleteAsync(chatId, ChatTypeConverter.Convert(ChatType.Group), CancellationToken.None);
                throw ex;
            }
            finally
            {
                if (completed)
                    await _messagePublisher.PublishAsync(new ChatDeletedMessage()
                    {
                        ChatId = chatId,
                        ChatType = ChatType.Group,
                        UserId = users
                    }, CancellationToken.None);
            }
        }

        public async Task DeleteFileFromMessageAsync(Guid userId, Guid chatId, Guid messageId, string mediaLink, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(mediaLink[(_mediaPrefix.Length + 1)..], out Guid mediaId))
                throw new DataValidationException("mediaLink");

            Guid[] users = (await _publicChatStore.GetChatFullInfoAsync(chatId, userId, cancellationToken))
                    .Members.Select(m => m.UserId).ToArray();
            bool completed = false;

            try
            {
                await _messagesService.DeleteAttachmentAsync(userId, chatId, ChatType.Group, messageId, mediaId, cancellationToken);
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
                            ChatType = ChatType.Group,
                            UserId = users,
                            File = $"{_mediaPrefix}{mediaId}",
                            MessageId = messageId
                        },
                        CancellationToken.None);
                    await _messageCacheService.InvalidateAsync(messageId, chatId, CancellationToken.None);
                }
            }
        }

        public async Task DeleteMessageAsync(Guid userId, Guid chatId, Guid messageId, CancellationToken cancellationToken)
        {
            Guid[] users = (await _publicChatStore.GetChatFullInfoAsync(chatId, userId, cancellationToken))
                    .Members.Select(m => m.UserId).ToArray();
            bool completed = false;

            try
            {
                await _messagesService.DeleteMessageAsync(userId, ChatType.Group, chatId, messageId, cancellationToken);
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
                            ChatType = ChatType.Group,
                            UserId = users,
                            MessageId = messageId
                        },
                        CancellationToken.None);
                    await _messageCacheService.InvalidateAsync(messageId, chatId, CancellationToken.None);
                }
            }
        }

        public async Task EditMessageTextAsync(Guid userId, UpdatingMessage updatingMessage, CancellationToken cancellationToken)
        {
            Guid[] users = (await _publicChatStore.GetChatFullInfoAsync(updatingMessage.ChatId, userId, cancellationToken))
                    .Members.Select(m => m.UserId).ToArray();
            bool completed = false;

            try
            {
                await _messagesService.UpdateMessageTextAsync(userId, ChatType.Group, updatingMessage, cancellationToken);
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
                            ChatType = ChatType.Group,
                            UserId = users,
                            MessageId = updatingMessage.MessageId
                        },
                        CancellationToken.None);
                    await _messageCacheService.InvalidateAsync(updatingMessage.MessageId, updatingMessage.ChatId, CancellationToken.None);
                }
            }
        }

        public async Task<Models.Output.PublicChatBannedUser[]> GetBannedUsersAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            return (await _publicChatStore.GetBannedUsersAsync(chatId, userId, cancellationToken))
                .Select(bu => new Models.Output.PublicChatBannedUser(bu)).ToArray();
        }

        public async Task<PublicChatFullInfo> GetChatFullInfoAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            return new PublicChatFullInfo(await _publicChatStore.GetChatFullInfoAsync(chatId, userId, cancellationToken), _mediaPrefix);
        }

        public async Task<ChatShortInfo> GetChatInfoAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            return new ChatShortInfo(await _publicChatStore.GetChatShortInfoAsync(chatId, userId, cancellationToken), _mediaPrefix);
        }

        public async Task<Models.Input.PublicChatOptions> GetChatOptionsAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            return new Models.Input.PublicChatOptions(await _publicChatStore.GetChatOptionsAsync(chatId, userId, cancellationToken), _mediaPrefix);
        }

        public async Task<PublicChatShortInfo[]> GetChatsByNameAsync(string chatName, Guid userId, PageOptions pageOptions, CancellationToken cancellationToken)
        {
            PublicChatFullInformation[] publicChats = await _publicChatStore.SearchChatsAsync(chatName, userId, pageOptions.Page, pageOptions.PageSize, cancellationToken);
            PublicChatShortInfo[] result = publicChats.Select(c => new PublicChatShortInfo(c, _mediaPrefix)).ToArray();
            return result;
        }

        public async Task<Models.Output.Message> GetMessageAsync(Guid userId, Guid chatId, Guid messageId, CancellationToken cancellationToken)
        {
            Models.Output.Message? message = await _messageCacheService.GetAsync(messageId, chatId, cancellationToken);
            if (message is null)
            {                
                message = await _messagesService.GetMessageByIdAsync(userId, ChatType.Group, chatId, messageId, cancellationToken);
                await _messageCacheService.SaveAsync(message, cancellationToken);
            }
            return message;
        }

        public async Task<Models.Output.Message[]> GetMessagesAsync(Guid userId, Guid chatId, MessagesSelectOptions options, CancellationToken cancellationToken)
        {
            return await _messagesService.GetMessagesAsync(userId, ChatType.Group, chatId, options, cancellationToken);
        }

        public async Task GiveMemberRoleAsync(Guid userId, Guid chatId, Guid memberId, PublicChatMemberRole newRole, CancellationToken cancellationToken)
        {
            await _publicChatStore.GiveMemberRoleAsync(memberId, chatId, userId, ChatRoleConverter.Convert(newRole), cancellationToken);
        }

        public async Task HandleUserTypingEventAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            Guid[] users = (await _publicChatStore.GetChatFullInfoAsync(chatId, userId, cancellationToken))
                    .Members.Select(m => m.UserId).Where(id => id != userId).ToArray();
            await _messagePublisher.PublishAsync(
                new UserIsTypingMessage()
                {
                    ChatId = chatId,
                    ChatType = ChatType.Group,
                    TypingUserId = userId,
                    DestinationUserId = users
                },
                cancellationToken);
        }

        public async Task JoinChatAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            await _publicChatStore.JoinChatAsync(chatId, userId, cancellationToken);
        }

        public async Task LeaveChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            await _publicChatStore.LeaveChatAsync(chatId, userId, cancellationToken);
        }

        public async Task RemoveAndBanChatMemberAsync(Guid userId, Guid chatId, Guid memberId, CancellationToken cancellationToken)
        {
            await _publicChatStore.DeleteAndBanChatMemberAsync(chatId, memberId, userId, cancellationToken);
        }

        public async Task RemoveMemberAsync(Guid userId, Guid chatId, Guid memberId, CancellationToken cancellationToken)
        {
            await _publicChatStore.DeleteChatMemberAsync(chatId, memberId, userId, cancellationToken);
        }

        public async Task<Guid[]> ResendMessagesAsync(Guid userId, ResendMessagesModel resendMessagesModel, CancellationToken cancellationToken)
        {
            Guid[] users = (await _publicChatStore.GetChatFullInfoAsync(resendMessagesModel.ChatId, userId, cancellationToken))
                    .Members.Select(m => m.UserId).ToArray();
            
            Guid[] ids = Array.Empty<Guid>();
            bool completed = false;

            try
            {
                ids = await _messagesService.ResendMessagesAsync(userId, ChatType.Group, resendMessagesModel, cancellationToken);
                completed = true;
            }
            finally
            {
                if (completed)
                    await _messagePublisher.PublishAsync(
                        new MessagesSentMessage()
                        {
                            ChatId = resendMessagesModel.ChatId,
                            ChatType = ChatType.Group,
                            UserId = users,
                            MessagesId = ids,
                        },
                        CancellationToken.None);
            }
            return ids;
        }

        public async Task<Guid> SendMessageAsync(SendingMessage sendingMessage, CancellationToken cancellationToken)
        {
            Guid[] users = (await _publicChatStore.GetChatFullInfoAsync(sendingMessage.ChatId, sendingMessage.Author, cancellationToken))
                    .Members.Select(m => m.UserId).ToArray();

            Guid id = Guid.Empty;
            bool completed = false;

            try
            {
                id = await _messagesService.SendUserMessageAsync(ChatType.Group, sendingMessage, cancellationToken);
                completed = true;
            }
            finally
            {
                if (completed)
                    await _messagePublisher.PublishAsync(
                        new MessagesSentMessage()
                        {
                            ChatId = sendingMessage.ChatId,
                            ChatType = ChatType.Group,
                            UserId = users,
                            MessagesId = [id]
                        },
                        CancellationToken.None);
            }
            return id;
        }

        public async Task UnbanUserAsync(Guid userId, Guid chatId, Guid unbannedUserId, CancellationToken cancellationToken)
        {
            await _publicChatStore.UnbanUserAsync(chatId, unbannedUserId, userId, cancellationToken);
        }

        public async Task UpdateChatAsync(Guid userId, Guid chatId, string? newName, bool? isSearchable, bool updateAvatar, FileUpload? newAvatar, PublicChatMemberRole? defaultRole, CancellationToken cancellationToken)
        {
            Guid? newAvatarId = newAvatar is not null ? Guid.NewGuid() : null;
            bool completed = false;
            try
            {
                await _publicChatStore.UpdateChatAsync(chatId, userId, newName, isSearchable, updateAvatar, newAvatar?.ToMediaFile(newAvatarId!.Value), (defaultRole.HasValue ? ChatRoleConverter.Convert(defaultRole.Value) : null), cancellationToken);
                completed = true;
            }
            finally
            {
                if (completed && updateAvatar && newAvatar is not null && newAvatarId.HasValue)
                {
                    await _objectStorage.SaveAsync(newAvatar.Content, newAvatarId.Value, CancellationToken.None);
                }
            }
        }
    }
}
