namespace Domain.Models.Types
{
    public record Message
    {
        public required Guid MessageId { get; init; }
        public required Guid Author { get; init; }
        public required string MessageText { get; init; }
        public required DateTime SentAt { get; init; }
        public required bool IsUpdated { get; init; }
        public required DateTime? UpdatedAt { get; init; }
        public required Guid? ReplyTo { get; init; }
        public required Guid? ResentFrom { get; init; }
        public required bool? IsBotResend { get; init; }
        public required MediaFile[] AttachedMedia { get; init; }
    }
}
