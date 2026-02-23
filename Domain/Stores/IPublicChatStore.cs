using Domain.Models.Types;

namespace Domain.Stores
{
    public interface IPublicChatStore
    {
        public Task<PublicChatFullInformation[]> SearchChatsAsync(string namePart);
        public Task<PublicChatFullInformation> GetChatFullInfoAsync(Guid chatId, Guid userId);
        public Task<object> GetChatShortInfoAsync(Guid chatId, Guid userId);///////////impl
        public Task<Guid[]> GetBannedUsersAsync(Guid chatId, Guid gettingBy);///////////impl
        public Task<Message[]> GetMessagesAsync(Guid chatId, Guid gettingBy, uint messagesCount, DateTime sentBefore);
        public Task<Message> GetMessageAsync(Guid chatId, Guid messageId, Guid gettingBy);///////////impl
        public Task<object> AuditChatAsync(Guid chatId, Guid gettingBy, uint pageNumber, uint pageSize);///////////impl
        public Task<Guid> CreateNewChatAsync(string chatName, Guid creatorId, bool isSearchable, MediaFile avatar, EnPublicChatMemberRole defaultMemberRole);///////////rmk
        public Task JoinChatAsync(Guid chatId, Guid userId);///////////impl
        public Task<Guid> SendMessageAsync(Guid chatId, Guid senderId, string text, MediaFile[]? attachments);///////////impl
        public Task UpdateMessageTextAsync(Guid chatId, Guid messageId, Guid senderId, string newText);
        public Task GiveMemberRoleAsync(Guid member, Guid chatId, Guid givingBy, EnPublicChatMemberRole newRole);
        public Task UpdateChatAsync(Guid chatId, Guid updatingBy, string? newName, bool? isSearchable, MediaFile? newAvatar);
        public Task BanUserAsync(Guid chatId, Guid userId, Guid banningBy);///////////impl
        public Task UnbanUserAsync(Guid chatId, Guid userId, Guid unbanningBy);///////////impl
        public Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid deletingBy);
        public Task DeleteFileFromMessageAsync(Guid chatId, Guid messageId, Guid fileId, Guid deletingBy);
        public Task LeaveChatAsync(Guid chatId, Guid userId);
        public Task DeleteChatMemberAsync(Guid chatId, Guid memberId, Guid deletingBy);///////////rmk
        public Task DeleteAndBanChatMemberAsync(Guid chatId, Guid memberId, Guid deletingBy);///////////impl
        public Task DeleteChatAsync(Guid chatId, Guid deletingBy);
    }
}