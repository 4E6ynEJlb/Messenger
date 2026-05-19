using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Internal.Constants;
using Application.Models.Output;

namespace Application.Services.Interfaces
{
    public interface IPublicChatService
    {
        public Task<PublicChatShortInfo[]> GetChatsByNameAsync(string chatName, Guid userId, PageOptions pageOptions, CancellationToken cancellationToken);
        public Task<ChatShortInfo> GetChatInfoAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
        public Task<PublicChatFullInfo> GetChatFullInfoAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
        public Task<PublicChatOptions> GetChatOptionsAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
        public Task<PublicChatBannedUser[]> GetBannedUsersAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
        public Task<Message[]> GetMessagesAsync(Guid userId, Guid chatId, MessagesSelectOptions options, CancellationToken cancellationToken);
        public Task<Message> GetMessageAsync(Guid userId, Guid chatId, Guid messageId, CancellationToken cancellationToken);
        public Task<PublicChatAuditRecord[]> AuditChatAsync(Guid userId, Guid chatId, PageOptions pageOptions, CancellationToken cancellationToken);
        public Task<Guid> CreateChatAsync(Guid creatorId, string name, bool isSearchable, FileUpload? avatar, PublicChatMemberRole defaultRole, CancellationToken cancellationToken);
        public Task JoinChatAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
        public Task<Guid> SendMessageAsync(Guid userId, SendingMessage sendingMessage, CancellationToken cancellationToken);
        public Task<Guid[]> ResendMessagesAsync(Guid userId, ResendMessagesModel resendMessagesModel, CancellationToken cancellationToken);
        public Task HandleUserTypingEventAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
        public Task EditMessageTextAsync(Guid userId, UpdatingMessage updatingMessage, CancellationToken cancellationToken);
        public Task GiveMemberRoleAsync(Guid userId, Guid chatId, Guid memberId, PublicChatMemberRole newRole, CancellationToken cancellationToken);
        public Task UpdateChatAsync(Guid userId, Guid chatId, string? newName, bool? isSearchable, bool updateAvatar, FileUpload? newAvatar, PublicChatMemberRole? defaultRole, CancellationToken cancellationToken);
        public Task BanUserAsync(Guid userId, Guid chatId, Guid bannedUserId, CancellationToken cancellationToken);
        public Task UnbanUserAsync(Guid userId, Guid chatId, Guid unbannedUserId, CancellationToken cancellationToken);
        public Task DeleteMessageAsync(Guid userId, Guid chatId, Guid messageId, CancellationToken cancellationToken);
        public Task DeleteFileFromMessageAsync(Guid userId, Guid chatId, Guid mediaId, CancellationToken cancellationToken);
        public Task LeaveChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
        public Task RemoveMemberAsync(Guid userId, Guid chatId, Guid memberId, CancellationToken cancellationToken);
        public Task RemoveAndBanChatMemberAsync(Guid userId, Guid chatId, Guid memberId, CancellationToken cancellationToken);
        public Task DeleteChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken);
    }
}
