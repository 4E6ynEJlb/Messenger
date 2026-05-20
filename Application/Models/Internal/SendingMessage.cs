using Application.Models.Input;

namespace Application.Models.Internal
{
    public record SendingMessage
    {
        public required Guid ChatId { get; init; }
        public required string? MessageText { get; init; }
        public required Guid? ReplyTo { get; init; }
        public required Guid Author { get; init; }
        public required FileUpload[]? Attachments { get; init; }
    }
}
