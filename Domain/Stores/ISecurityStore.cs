using Domain.Models.Types;

namespace Domain.Stores
{
    public interface ISecurityStore
    {
        public Task ReportMessage(Guid reportedBy, EnChatType chatType, Guid chatId, Guid messageId, string? comment);
        public Task ReportUser(Guid reportedBy, Guid reportedUserId, string? comment);
        public Task ReportBot(Guid reportedBy, Guid botId, string? comment);
        public Task ReportPublicChat(Guid reportedBy, Guid chatId, string? comment);
        public Task ReportAdministrator(Guid reportedBy, int adminId, string? comment);
    }
}
