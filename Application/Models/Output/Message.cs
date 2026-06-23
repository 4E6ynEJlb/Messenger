using Application.Models.Input;
using Domain.Models.Documents;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Output
{
    /// <summary>
    /// Mtssage output model
    /// </summary>
    public record Message : UpdatingMessage
    {
        public Message() { }

        [SetsRequiredMembers]
        public Message(Domain.Models.Types.Message message, string mediaPrefix, Guid chatId)
        {
            Author = message.Author;
            SentAt = message.SentAt;
            IsUpdated = message.IsUpdated;
            UpdatedAt = message.UpdatedAt;
            ReplyTo = message.ReplyTo;
            ResentFrom = message.ResentFrom;
            IsBotResend = message.IsBotResend;
            AttachedMedia = message.AttachedMedia.Select(am => $"{mediaPrefix}/{am}").ToArray();
            MessageId = message.MessageId;
            ChatId = chatId;
            MessageText = message.MessageText;
        }

        [SetsRequiredMembers]
        public Message(Domain.Models.Types.Message message, string mediaPrefix, Guid chatId, Guid[] media)
        {
            Author = message.Author;
            SentAt = message.SentAt;
            IsUpdated = message.IsUpdated;
            UpdatedAt = message.UpdatedAt;
            ReplyTo = message.ReplyTo;
            ResentFrom = message.ResentFrom;
            IsBotResend = message.IsBotResend;
            AttachedMedia = media.Select(am => $"{mediaPrefix}/{am}").ToArray();
            MessageId = message.MessageId;
            ChatId = chatId;
            MessageText = message.MessageText;
        }

        [SetsRequiredMembers]
        public Message(NewMessage message, string mediaPrefix)
        {
            Author = message.Author;
            SentAt = message.SentAt;
            IsUpdated = message.IsUpdated;
            UpdatedAt = message.UpdatedAt;
            ReplyTo = message.ReplyTo;
            ResentFrom = message.ResentFrom;
            IsBotResend = message.IsBotResend;
            AttachedMedia = message.AttachedMedia.Select(am => $"{mediaPrefix}/{am}").ToArray();
            MessageId = message.MessageId;
            ChatId = message.ChatId;
            MessageText = message.MessageText;
        }

        [SetsRequiredMembers]
        public Message(NewMessage message, string mediaPrefix, Guid[] media)
        {
            Author = message.Author;
            SentAt = message.SentAt;
            IsUpdated = message.IsUpdated;
            UpdatedAt = message.UpdatedAt;
            ReplyTo = message.ReplyTo;
            ResentFrom = message.ResentFrom;
            IsBotResend = message.IsBotResend;
            AttachedMedia = media.Select(am => $"{mediaPrefix}/{am}").ToArray();
            MessageId = message.MessageId;
            ChatId = message.ChatId;
            MessageText = message.MessageText;
        }
        /// <summary>
        /// Author user id
        /// </summary>
        public required Guid Author { get; set; }
        /// <summary>
        /// Time when the message was sent
        /// </summary>
        public required DateTime SentAt { get; set; }
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
        public required Guid? ReplyTo { get; set; }
        /// <summary>
        /// If message is resend of another message then this field will contain id of the author
        /// </summary>
        public required Guid? ResentFrom { get; set; }
        /// <summary>
        /// If message is resend, then true means that the author is bot, false - user
        /// </summary>
        public required bool? IsBotResend { get; set; }
        /// <summary>
        /// Links to attached media files
        /// </summary>
        public required string[] AttachedMedia { get; init; }
    }
}
