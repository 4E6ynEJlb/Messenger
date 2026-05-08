using Application.Models.Internal;

namespace Application.Models.Input
{
    public record SendMessageForm
    {
        public required Guid ChatId { get; init; }
        public required string? MessageText { get; init; }
        public required Guid? ReplyTo { get; init; }
        public required ResendingMessagesOptions? ResendingMessages { get; init; }
        public required IFormFile[] Attachments { get; init; }
    }
}
