using Application.Models.Internal.Constants;
using Application.Models.OptionsAndHelpers;
using Application.Services.Interfaces;
using Domain.Stores;

namespace Application.Services.Implementations
{
    public class SecurityService : ISecurityService
    {
        private readonly ISecurityStore _securityStore;
        public SecurityService(ISecurityStore securityStore)
        {
            _securityStore = securityStore;
        }

        public async Task ReportAdministrator(Guid userId, int adminId, string? comment, CancellationToken cancellationToken)
        {
            await _securityStore.ReportAdministratorAsync(userId, adminId, comment, cancellationToken);
        }

        public async Task ReportBotAsync(Guid userId, Guid botId, string? comment, CancellationToken cancellationToken)
        {
            await _securityStore.ReportBotAsync(userId, botId, comment, cancellationToken);
        }

        public async Task ReportMessageAsync(Guid userId, ChatType chatType, Guid chatId, Guid messageId, string? comment, CancellationToken cancellationToken)
        {
            await _securityStore.ReportMessageAsync(userId, ChatTypeConverter.Convert(chatType), chatId, messageId, comment, cancellationToken);
        }

        public async Task ReportPublicChatAsync(Guid userId, Guid chatId, string? comment, CancellationToken cancellationToken)
        {
            await _securityStore.ReportPublicChatAsync(userId, chatId, comment, cancellationToken);
        }

        public async Task ReportUserAsync(Guid userId, Guid reportedUserId, string? comment, CancellationToken cancellationToken)
        {
            await _securityStore.ReportUserAsync(userId, reportedUserId, comment, cancellationToken);
        }
    }
}
