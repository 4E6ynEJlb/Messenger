using Application.Models.Internal;

namespace Application.Models.Input
{
    public record SendMessageForm
    {
        public required Guid ChatId { get; init; }
        /// <summary>
        /// Not null if attachments field is empty
        /// </summary>
        public string? MessageText { get; init; }
        /// <summary>
        /// Message id for replying. Should be null if the message is not a reply.
        /// </summary>
        public Guid? ReplyTo { get; init; }
        public IFormFile[]? Attachments { get; init; }
    }
}
