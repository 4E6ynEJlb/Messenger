using Application.Models.Internal;

namespace Application.Models.Output
{
    public record PublicChatAuditRecord
    {
        public required Guid UserId { get; init; }
        public required Guid? TargetUserId { get; init; }
        public required DateTime ActionDate { get; init; }
        public required AuditRecordAction Action { get; init; }
    }
}
