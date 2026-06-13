using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.Documents
{
    public record PublicMessageUpdate
    {
        [BsonId]
        public Guid UpdateId { get; init; }

        [BsonElement("chatId")]
        public required Guid ChatId { get; init; }

        [BsonElement("updatedBy")]
        public required Guid UpdatedBy { get; init; }
        
        [BsonElement("updatedAt")]
        public required DateTime UpdatedAt { get; init; }
    }
}
