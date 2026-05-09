using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Database;

namespace Persistence.Repositories
{
    public class PersonalChatRepository : IPersonalChatStore
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public PersonalChatRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task BlockUserAsync(Guid blockingBy, Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateChatAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteChatAsync(Guid chatId, Guid deletingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileFromMessageAsync(Guid chatId, Guid attachmentId, Guid deletingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid deletingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ChatInformation> GetChatShortInfoAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Message> GetMessageAsync(Guid chatId, Guid messageId, Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Message[]> GetMessagesAsync(Guid chatId, Guid gettingBy, uint messagesCount, DateTime sentBefore, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> GetUserIdByChatIdAsync(Guid chatId, Guid gettingBy, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid[]> ResendMessagesAsync(Guid chatId, Guid senderId, EnChatType sourceChatType, Guid sourceChatId, Guid[] messages, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> SendMessageAsync(Guid chatId, Guid senderId, Guid? replyTo, string text, MediaFile[]? attachments, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UnblockUserAsync(Guid unblockingBy, Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMessageTextAsync(Guid chatId, Guid messageId, Guid senderId, string newText, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
