using Application.Models.Helpers;
using Application.Models.Internal.Constants;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Output
{
    /// <summary>
    /// Event from public chat audit log
    /// </summary>
    public record PublicChatAuditRecord
    {
        public PublicChatAuditRecord() { }
        [SetsRequiredMembers]
        public PublicChatAuditRecord(Domain.Models.Types.AuditLogRecord auditLogRecord)
        {
            UserId = auditLogRecord.SourceUserId;
            TargetUserId = auditLogRecord.DestinationUserId;
            ActionDate = auditLogRecord.ActionDatetime;
            Action = AuditActionConverter.Convert(auditLogRecord.Action);
        }
        /// <summary>
        /// Source user id. 
        /// For example if user A added user B to chat 
        /// then source user id will be A and target user id will be B
        /// </summary>
        public required Guid UserId { get; init; }
        /// <summary>
        /// Null if action is not related to specific user. 
        /// For example if user A changed chat name then source user id 
        /// will be A and target user id will be null
        /// </summary>
        public required Guid? TargetUserId { get; init; }
        public required DateTime ActionDate { get; init; }
        public required AuditRecordAction Action { get; init; }
    }
}
