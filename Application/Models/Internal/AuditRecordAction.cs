namespace Application.Models.Internal
{
    /// <summary>
    /// 0 - Join;
    /// 1 - UpdateMessage;
    /// 2 - ChangeRole;
    /// 3 - UpdateSettings;
    /// 4 - Ban;
    /// 5 - Unban;
    /// 6 - DeleteMessage;
    /// 7 - DeleteAttachment;
    /// 8 - Leave;
    /// 9 - Kick.
    /// </summary>
    public enum AuditRecordAction
    {
        Join,
        UpdateMessage,
        ChangeRole,
        UpdateSettings,
        Ban,
        Unban,
        DeleteMessage,
        DeleteAttachment,
        Leave,
        Kick
    }
}
