using Domain.Models.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.Documents
{
    public record NewMessage
    {
        [BsonId]
        public Guid MessageId { get; init; }

        [BsonElement("chatId")]
        public required Guid ChatId { get; init; }

        [BsonElement("chatType")]
        [BsonRepresentation(BsonType.String)]
        public required EnChatType ChatType { get; init; }

        [BsonElement("author")]
        public required Guid Author { get; init; }

        [BsonElement("isBot")]
        public required bool IsBot { get; init; }

        [BsonElement("messageText")]
        [BsonIgnoreIfDefault]
        public required string? MessageText { get; set; }

        [BsonElement("sentAt")]
        public required DateTime SentAt { get; init; }

        [BsonElement("isUpdated")]
        public required bool IsUpdated { get; set; }

        [BsonElement("updatedAt")]
        [BsonIgnoreIfDefault]
        public required DateTime? UpdatedAt { get; set; }

        [BsonElement("replyTo")]
        [BsonIgnoreIfDefault]
        public required Guid? ReplyTo { get; init; }

        [BsonElement("resentFrom")]
        [BsonIgnoreIfDefault]
        public required Guid? ResentFrom { get; init; }

        [BsonElement("isBotResend")]
        [BsonIgnoreIfDefault]
        public required bool? IsBotResend { get; init; }

        [BsonElement("attachedMedia")]
        public required List<Guid> AttachedMedia { get; set; }
    }
}
