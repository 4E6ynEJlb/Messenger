using Application.Models.Input;

namespace Application.Models.Output
{
    /// <summary>
    /// Mtssage output model
    /// </summary>
    public record Message : UpdatingMessage
    {
        /// <summary>
        /// Author user id
        /// </summary>
        public required Guid Author { get; init; }
        /// <summary>
        /// Time when the message was sent
        /// </summary>
        public required DateTime SentAt { get; init; }
        public required bool IsUpdated { get; init; }
        /// <summary>
        /// If IsUpdated=true then this field will contain 
        /// time when the message was updated for the last time
        /// </summary>
        public required DateTime? UpdatedAt { get; init; }
        /// <summary>
        /// Message id of the message that this message is replying to. 
        /// Can be null if the message is not a reply
        /// </summary>
        public required Guid? ReplyTo { get; init; }
        /// <summary>
        /// If message is resend of another message then this field will contain id of the author
        /// </summary>
        public required Guid? ResentFrom { get; init; }
        /// <summary>
        /// If message is resend, then true means that the author is bot, false - user
        /// </summary>
        public required bool? IsBotResend { get; init; }
        /// <summary>
        /// Links to attached media files
        /// </summary>
        public required string[] AttachedMedia { get; init; }
    }
}
