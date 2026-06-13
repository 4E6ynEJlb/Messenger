using Domain.Models.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.Documents
{
    public record DeletedMessage
    {
        [BsonId]
        public Guid MessageId { get; init; }

        [BsonElement("chatId")]
        public required Guid ChatId { get; init; }

        [BsonElement("chatType")]
        [BsonRepresentation(BsonType.String)]
        public required EnChatType ChatType { get; init; }

        [BsonElement("deletedBy")]
        public required Guid DeletedBy { get; init; }

        [BsonElement("deletedAt")]
        public required DateTime DeletedAt { get; init; }
    }
}
