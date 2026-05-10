namespace Domain.Models.Types
{
    public record AuditLogRecord
    {
        public required DateTime ActionDatetime { get; init; }
        public required Guid SourceUserId { get; init; }
        public required Guid DestinationUserId { get; init; }
        public required EnPublicChatAuditRecordAction Action { get; init; }
    }
}
