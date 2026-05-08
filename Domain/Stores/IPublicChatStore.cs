using Domain.Models.Types;

namespace Domain.Stores
{
    public interface IPublicChatStore
    {
        public Task<PublicChatFullInformation[]> SearchChatsAsync(string namePart, Guid gettingBy, uint pageNumber, uint pageSize);
        public Task<PublicChatFullInformation> GetChatFullInfoAsync(Guid chatId, Guid userId);
        public Task<ChatInformation> GetChatShortInfoAsync(Guid chatId, Guid userId);
        public Task<PublicChatOptions> GetChatOptionsAsync(Guid chatId, Guid gettingBy);
        public Task<PublicChatBannedUser[]> GetBannedUsersAsync(Guid chatId, Guid gettingBy);
        public Task<Message[]> GetMessagesAsync(Guid chatId, Guid gettingBy, uint messagesCount, DateTime sentBefore);
        public Task<Message> GetMessageAsync(Guid chatId, Guid messageId, Guid gettingBy);
        public Task<AuditLogRecord[]> AuditChatAsync(Guid chatId, Guid gettingBy, uint pageNumber, uint pageSize);
        public Task<Guid> CreateNewChatAsync(string chatName, Guid creatorId, bool isSearchable, MediaFile avatar, EnPublicChatMemberRole defaultMemberRole);
        public Task JoinChatAsync(Guid chatId, Guid userId);
        public Task<Guid> SendMessageAsync(Guid chatId, Guid senderId, Guid? replyTo, string text, MediaFile[]? attachments);
        public Task<Guid[]> ResendMessagesAsync(Guid chatId, Guid senderId, EnChatType sourceChatType, Guid sourceChatId, Guid[] messages);
        public Task UpdateMessageTextAsync(Guid chatId, Guid messageId, Guid senderId, string newText);
        public Task GiveMemberRoleAsync(Guid member, Guid chatId, Guid givingBy, EnPublicChatMemberRole newRole);
        public Task UpdateChatAsync(Guid chatId, Guid updatingBy, string? newName, bool? isSearchable, MediaFile? newAvatar, EnPublicChatMemberRole defaultMemberRole);
        public Task BanUserAsync(Guid chatId, Guid userId, Guid banningBy);
        public Task UnbanUserAsync(Guid chatId, Guid userId, Guid unbanningBy);
        public Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid deletingBy);
        public Task DeleteFileFromMessageAsync(Guid chatId, Guid messageId, Guid fileId, Guid deletingBy);
        public Task LeaveChatAsync(Guid chatId, Guid userId);
        public Task DeleteChatMemberAsync(Guid chatId, Guid memberId, Guid deletingBy);
        public Task DeleteAndBanChatMemberAsync(Guid chatId, Guid memberId, Guid deletingBy);
        public Task DeleteChatAsync(Guid chatId, Guid deletingBy);
    }
}