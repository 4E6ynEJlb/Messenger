using Domain.Models.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.Documents
{
    public record MessageUpdate
    {
        [BsonId]
        public Guid UpdateId { get; init; }

        [BsonElement("messageId")]
        public Guid MessageId { get; init; }

        [BsonElement("chatId")]
        public required Guid ChatId { get; init; }

        [BsonElement("chatType")]
        [BsonRepresentation(BsonType.String)]
        public required EnChatType ChatType { get; init; }

        [BsonElement("updatedBy")]
        public required Guid UpdatedBy { get; init; }
        
        [BsonElement("updatedAt")]
        public required DateTime UpdatedAt { get; init; }
    }
}
