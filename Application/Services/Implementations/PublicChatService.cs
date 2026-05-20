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
using Infrastructure.Storage;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations
{
    public class PublicChatService : IPublicChatService
    {
        private readonly string _mediaPrefix;
        private readonly IPublicChatStore _publicChatStore;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IObjectStorage _objectStorage;
        private readonly IMessageCacheService _messageCacheService;
        public PublicChatService(
            IPublicChatStore publicChatStore, IMessagePublisher messagePublisher,
            IObjectStorage objectStorage, IMessageCacheService messageCacheService, IOptions<ApplicationServicesOptions> options)
        {
            _publicChatStore = publicChatStore;
            _messagePublisher = messagePublisher;
            _objectStorage = objectStorage;
            _messageCacheService = messageCacheService;
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
            Guid chatId = await _publicChatStore.CreateNewChatAsync(name, creatorId, isSearchable, avatar?.ToMediaFile(mediaId), ChatRoleConverter.Convert(defaultRole), cancellationToken);
            if (avatar is not null)
            {
                await _objectStorage.SaveAsync(avatar.Content, mediaId, cancellationToken);
            }
            return chatId;
        }

        public async Task DeleteChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            Guid[] users = (await _publicChatStore.GetChatFullInfoAsync(chatId, userId, cancellationToken))
                    .Members.Select(m => m.UserId).ToArray();
            await _publicChatStore.DeleteChatAsync(chatId, userId, cancellationToken);
            await _messagePublisher.PublishAsync(new ChatDeletedMessage() 
            { 
                ChatId = chatId, 
                ChatType = ChatType.Group, 
                UserId = users
            }, cancellationToken);
        }

        public async Task DeleteFileFromMessageAsync(Guid userId, Guid chatId, string mediaLink, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(mediaLink[(_mediaPrefix.Length + 1)..], out Guid mediaId))
                throw new DataValidationException("mediaLink");
            Guid[] users = (await _publicChatStore.GetChatFullInfoAsync(chatId, userId, cancellationToken))
                    .Members.Select(m => m.UserId).ToArray();
            Guid messageId = await _publicChatStore.GetMessageIdByMediaAsync(chatId, mediaId, cancellationToken);
            await _publicChatStore.DeleteFileFromMessageAsync(chatId, messageId, userId, mediaId, cancellationToken);
            await _messagePublisher.PublishAsync(
                new FileDeletedMessage()
                {
                    ChatId = chatId,
                    ChatType = ChatType.Group,
                    UserId = users,
                    File = $"{_mediaPrefix}{mediaId}",
                    MessageId = messageId
                },
                cancellationToken);
            await _messageCacheService.InvalidateAsync(messageId, chatId, cancellationToken);
        }

        public async Task DeleteMessageAsync(Guid userId, Guid chatId, Guid messageId, CancellationToken cancellationToken)
        {
            Guid[] users = (await _publicChatStore.GetChatFullInfoAsync(chatId, userId, cancellationToken))
                    .Members.Select(m => m.UserId).ToArray();
            await _publicChatStore.DeleteMessageAsync(chatId, messageId, userId, cancellationToken);
            await _messagePublisher.PublishAsync(
                new MessageDeletedMessage()
                {
                    ChatId = chatId,
                    ChatType = ChatType.Group,
                    UserId = users,
                    MessageId = messageId
                },
                cancellationToken);
            await _messageCacheService.InvalidateAsync(messageId, chatId, cancellationToken);
        }

        public async Task EditMessageTextAsync(Guid userId, UpdatingMessage updatingMessage, CancellationToken cancellationToken)
        {
            Guid[] users = (await _publicChatStore.GetChatFullInfoAsync(updatingMessage.ChatId, userId, cancellationToken))
                    .Members.Select(m => m.UserId).ToArray();
            await _publicChatStore.UpdateMessageTextAsync(updatingMessage.ChatId, updatingMessage.MessageId, userId, updatingMessage.MessageText, cancellationToken);
            await _messagePublisher.PublishAsync(
                new MessageUpdatedMessage()
                {
                    ChatId = updatingMessage.ChatId,
                    ChatType = ChatType.Group,
                    UserId = users,
                    MessageId = updatingMessage.MessageId
                },
                cancellationToken);
            await _messageCacheService.InvalidateAsync(updatingMessage.MessageId, updatingMessage.ChatId, cancellationToken);
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
                Domain.Models.Types.Message messageData = await _publicChatStore.GetMessageAsync(chatId, messageId, userId, cancellationToken);
                message = new Models.Output.Message(messageData, _mediaPrefix, chatId);
                await _messageCacheService.SaveAsync(message, cancellationToken);
            }
            return message;
        }

        public async Task<Models.Output.Message[]> GetMessagesAsync(Guid userId, Guid chatId, MessagesSelectOptions options, CancellationToken cancellationToken)
        {
            Domain.Models.Types.Message[] messages = await _publicChatStore.GetMessagesAsync(chatId, userId, options.MessagesCount, options.SentBefore ?? DateTime.UtcNow, cancellationToken);
            return messages.Select(m => new Models.Output.Message(m, _mediaPrefix, chatId)).ToArray();
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
            Guid[] ids = await _publicChatStore.ResendMessagesAsync(resendMessagesModel.ChatId, userId, ChatTypeConverter.Convert(resendMessagesModel.SourceChatType), resendMessagesModel.SourceChatId, resendMessagesModel.Messages, cancellationToken);
            await _messagePublisher.PublishAsync(
                new MessagesSentMessage()
                {
                    ChatId = resendMessagesModel.ChatId,
                    ChatType = ChatType.Group,
                    UserId = users,
                    MessagesId = ids,
                },
                cancellationToken);
            return ids;
        }

        public async Task<Guid> SendMessageAsync(SendingMessage sendingMessage, CancellationToken cancellationToken)
        {
            MediaFile[]? attachments = null;
            if (sendingMessage.Attachments != null && sendingMessage.Attachments.Length > 0)
            {
                var list = new List<MediaFile>(sendingMessage.Attachments.Length);
                foreach (var a in sendingMessage.Attachments)
                {
                    var mediaId = Guid.NewGuid();
                    await _objectStorage.SaveAsync(a.Content, mediaId, cancellationToken);
                    list.Add(a.ToMediaFile(mediaId));
                }
                attachments = list.ToArray();
            }

            Guid[] users = (await _publicChatStore.GetChatFullInfoAsync(sendingMessage.ChatId, sendingMessage.Author, cancellationToken))
                    .Members.Select(m => m.UserId).ToArray();

            Guid id = await _publicChatStore.SendMessageAsync(
                sendingMessage.ChatId, sendingMessage.Author,
                sendingMessage.ReplyTo, sendingMessage.MessageText,
                attachments, cancellationToken);

            await _messagePublisher.PublishAsync(
                new MessagesSentMessage()
                {
                    ChatId = sendingMessage.ChatId,
                    ChatType = ChatType.Group,
                    UserId = users,
                    MessagesId = [id]
                },
                cancellationToken);

            return id;
        }

        public async Task UnbanUserAsync(Guid userId, Guid chatId, Guid unbannedUserId, CancellationToken cancellationToken)
        {
            await _publicChatStore.UnbanUserAsync(chatId, unbannedUserId, userId, cancellationToken);
        }

        public async Task UpdateChatAsync(Guid userId, Guid chatId, string? newName, bool? isSearchable, bool updateAvatar, FileUpload? newAvatar, PublicChatMemberRole? defaultRole, CancellationToken cancellationToken)
        {
            Guid? newAvatarId = newAvatar is not null ? Guid.NewGuid() : null;
            await _publicChatStore.UpdateChatAsync(chatId, userId, newName, isSearchable, updateAvatar, newAvatar?.ToMediaFile(Guid.NewGuid()), (defaultRole.HasValue ? ChatRoleConverter.Convert(defaultRole.Value) : null), cancellationToken);
            if (updateAvatar && newAvatar is not null && newAvatarId.HasValue)
            {
                await _objectStorage.SaveAsync(newAvatar.Content, newAvatarId.Value, cancellationToken);
            }
        }
    }
}
