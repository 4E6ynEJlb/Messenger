using Application.Models.Internal;

namespace Application.Models.Input
{
    public record SendMessageForm
    {
        public required Guid ChatId { get; init; }
        /// <summary>
        /// Not null if attachments field is empty
        /// </summary>
        public required string? MessageText { get; init; }
        /// <summary>
        /// Message id for replying. Should be null if the message is not a reply.
        /// </summary>
        public required Guid? ReplyTo { get; init; }
        public required IFormFile[] Attachments { get; init; }
    }
}
