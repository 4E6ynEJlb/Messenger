using Application.Models.Input;

namespace Application.Models.Internal
{
    public record SendingMessage : UpdatingMessage
    {
        public required Guid? ReplyTo { get; init; }
        public required ResendingMessagesOptions? ResendingMessages { get; init; }
        public required FileUpload[] Attachments { get; init; }
    }
}
