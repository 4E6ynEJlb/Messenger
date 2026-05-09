using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Database;

namespace Persistence.Repositories
{
    public class PublicChatRepository : IPublicChatStore
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public PublicChatRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<AuditLogRecord[]> AuditChatAsync(Guid chatId, Guid gettingBy, uint pageNumber, uint pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task BanUserAsync(Guid chatId, Guid userId, Guid banningBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateNewChatAsync(string chatName, Guid creatorId, bool isSearchable, MediaFile avatar, EnPublicChatMemberRole defaultMemberRole, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAndBanChatMemberAsync(Guid chatId, Guid memberId, Guid deletingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteChatAsync(Guid chatId, Guid deletingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteChatMemberAsync(Guid chatId, Guid memberId, Guid deletingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileFromMessageAsync(Guid chatId, Guid messageId, Guid fileId, Guid deletingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid deletingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<PublicChatBannedUser[]> GetBannedUsersAsync(Guid chatId, Guid gettingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<PublicChatFullInformation> GetChatFullInfoAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<PublicChatOptions> GetChatOptionsAsync(Guid chatId, Guid gettingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ChatInformation> GetChatShortInfoAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Message> GetMessageAsync(Guid chatId, Guid messageId, Guid gettingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Message[]> GetMessagesAsync(Guid chatId, Guid gettingBy, uint messagesCount, DateTime sentBefore, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task GiveMemberRoleAsync(Guid member, Guid chatId, Guid givingBy, EnPublicChatMemberRole newRole, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task JoinChatAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task LeaveChatAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid[]> ResendMessagesAsync(Guid chatId, Guid senderId, EnChatType sourceChatType, Guid sourceChatId, Guid[] messages, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<PublicChatFullInformation[]> SearchChatsAsync(string namePart, Guid gettingBy, uint pageNumber, uint pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> SendMessageAsync(Guid chatId, Guid senderId, Guid? replyTo, string text, MediaFile[]? attachments, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UnbanUserAsync(Guid chatId, Guid userId, Guid unbanningBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateChatAsync(Guid chatId, Guid updatingBy, string? newName, bool? isSearchable, bool updateAvatar, MediaFile? newAvatar, EnPublicChatMemberRole defaultMemberRole, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMessageTextAsync(Guid chatId, Guid messageId, Guid senderId, string newText, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
