using Application.Models.Internal.Constants;

namespace Application.Services.Interfaces
{
    public interface ISecurityService
    {
        public Task ReportMessageAsync(Guid userId, ChatType chatType, Guid chatId, Guid messageId, string? comment, CancellationToken cancellationToken);
        public Task ReportUserAsync(Guid userId, Guid reportedUserId, string? comment, CancellationToken cancellationToken);
        public Task ReportBotAsync(Guid userId, Guid botId, string? comment, CancellationToken cancellationToken);
        public Task ReportPublicChatAsync(Guid userId, Guid chatId, string? comment, CancellationToken cancellationToken);
        public Task ReportAdministrator(Guid userId, int adminId, string? comment, CancellationToken cancellationToken);
    }
}
