namespace Domain.Stores
{
    public interface ISecurityStore//////////impl
    {
        public Task ReportMessage(Guid reportedBy, object chatType, Guid chatId, Guid messageId, string? comment);
        public Task ReportUser(Guid reportedBy, Guid reportedUserId, string? comment);
        public Task ReportBot(Guid reportedBy, Guid botId, string? comment);
        public Task ReportPublicChat(Guid reportedBy, Guid chatId);
    }
}
