using Domain.Models.Types;

namespace Domain.Stores
{
    public interface ISecurityStore
    {
        public Task ReportMessageAsync(Guid reportedBy, EnChatType chatType, Guid chatId, Guid messageId, string? comment, CancellationToken cancellationToken);
        public Task ReportUserAsync(Guid reportedBy, Guid reportedUserId, string? comment, CancellationToken cancellationToken);
        public Task ReportBotAsync(Guid reportedBy, Guid botId, string? comment, CancellationToken cancellationToken);
        public Task ReportPublicChatAsync(Guid reportedBy, Guid chatId, string? comment, CancellationToken cancellationToken);
        public Task ReportAdministratorAsync(Guid reportedBy, int adminId, string? comment, CancellationToken cancellationToken);
    }
}
