using Domain.Models.Types;

namespace Domain.Stores
{
    public interface IPublicChatStore
    {
        public Task<PublicChatFullInformation[]> SearchChatsAsync(string namePart, Guid gettingBy, uint pageNumber, uint pageSize, CancellationToken cancellationToken);
        public Task<PublicChatFullInformation> GetChatFullInfoAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
        public Task<ChatInformation> GetChatShortInfoAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
        public Task<PublicChatOptions> GetChatOptionsAsync(Guid chatId, Guid gettingBy, CancellationToken cancellationToken);
        public Task<PublicChatBannedUser[]> GetBannedUsersAsync(Guid chatId, Guid gettingBy, CancellationToken cancellationToken);
        public Task<Message[]> GetMessagesAsync(Guid chatId, Guid gettingBy, uint messagesCount, DateTime sentBefore, CancellationToken cancellationToken);
        public Task<Message> GetMessageAsync(Guid chatId, Guid messageId, Guid gettingBy, CancellationToken cancellationToken);
        public Task<Guid> GetMessageIdByMediaAsync(Guid chatId, Guid mediaId, CancellationToken cancellationToken);
        public Task<AuditLogRecord[]> AuditChatAsync(Guid chatId, Guid gettingBy, uint pageNumber, uint pageSize, CancellationToken cancellationToken);
        public Task<Guid> CreateNewChatAsync(string chatName, Guid creatorId, bool isSearchable, MediaFile? avatar, EnPublicChatMemberRole defaultMemberRole, CancellationToken cancellationToken);
        public Task JoinChatAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
        public Task<bool> CheckMessageSendingAbilityAsync(Guid chatId, Guid senderId, Guid? replyTo, CancellationToken cancellationToken);
        public Task UpdateMessageTextAsync(Guid chatId, Guid messageId, Guid senderId, string? newText, CancellationToken cancellationToken);
        public Task GiveMemberRoleAsync(Guid member, Guid chatId, Guid givingBy, EnPublicChatMemberRole newRole, CancellationToken cancellationToken);
        public Task UpdateChatAsync(Guid chatId, Guid updatingBy, string? newName, bool? isSearchable, bool updateAvatar, MediaFile? newAvatar, EnPublicChatMemberRole? defaultMemberRole, CancellationToken cancellationToken);
        public Task BanUserAsync(Guid chatId, Guid userId, Guid banningBy, CancellationToken cancellationToken);
        public Task UnbanUserAsync(Guid chatId, Guid userId, Guid unbanningBy, CancellationToken cancellationToken);
        public Task LeaveChatAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
        public Task DeleteChatMemberAsync(Guid chatId, Guid memberId, Guid deletingBy, CancellationToken cancellationToken);
        public Task DeleteAndBanChatMemberAsync(Guid chatId, Guid memberId, Guid deletingBy, CancellationToken cancellationToken);
        public Task DeleteChatAsync(Guid chatId, Guid deletingBy, CancellationToken cancellationToken);
    }
}