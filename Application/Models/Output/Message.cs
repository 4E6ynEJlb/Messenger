using Application.Models.Input;

namespace Application.Models.Output
{
    public record Message : UpdatingMessage
    {
        public required Guid Author { get; init; }
        public required DateTime SentAt { get; init; }
        public required bool IsUpdated { get; init; }
        public required DateTime? UpdatedAt { get; init; }
        public required ReplyInfo? Reply { get; init; }
        public required Guid? ResentFrom { get; init; }
        public required string[] AttachedMedia { get; init; }
    }
}
