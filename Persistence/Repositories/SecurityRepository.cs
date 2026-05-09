using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Database;

namespace Persistence.Repositories
{
    public class SecurityRepository : ISecurityStore
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public SecurityRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task ReportAdministratorAsync(Guid reportedBy, int adminId, string? comment, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ReportBotAsync(Guid reportedBy, Guid botId, string? comment, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ReportMessageAsync(Guid reportedBy, EnChatType chatType, Guid chatId, Guid messageId, string? comment, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ReportPublicChatAsync(Guid reportedBy, Guid chatId, string? comment, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ReportUserAsync(Guid reportedBy, Guid reportedUserId, string? comment, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
