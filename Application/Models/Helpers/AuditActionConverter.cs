using Application.Models.Internal;
using Domain.Models.Types;

namespace Application.Models.Helpers
{
    internal static class AuditActionConverter
    {
        internal static AuditRecordAction Convert(EnPublicChatAuditRecordAction action) => action switch
        {
            EnPublicChatAuditRecordAction.Join => AuditRecordAction.Join,
            EnPublicChatAuditRecordAction.UpdateMessage => AuditRecordAction.UpdateMessage,
            EnPublicChatAuditRecordAction.ChangeRole => AuditRecordAction.ChangeRole,
            EnPublicChatAuditRecordAction.UpdateSettings => AuditRecordAction.UpdateSettings,
            EnPublicChatAuditRecordAction.Ban => AuditRecordAction.Ban,
            EnPublicChatAuditRecordAction.Unban => AuditRecordAction.Unban,
            EnPublicChatAuditRecordAction.Leave => AuditRecordAction.Leave,
            EnPublicChatAuditRecordAction.Kick => AuditRecordAction.Kick,
            EnPublicChatAuditRecordAction.DeleteMessage => AuditRecordAction.DeleteMessage,
            EnPublicChatAuditRecordAction.DeleteAttachment => AuditRecordAction.DeleteAttachment,
            _ => throw new Exception("Invalid audit record action")
        };
    }
}
