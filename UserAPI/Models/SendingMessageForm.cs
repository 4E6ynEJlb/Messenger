using Application.Models.Internal;

namespace Application.Models.Input
{
    public record SendingMessageForm : UpdatingMessage
    {
        public required Guid? ReplyTo { get; init; }
        public required ResendingMessagesOptions? ResendingMessages { get; init; }
        public required IFormFile[] Attachments { get; init; }
    }
}
