using Domain.Models.Documents.Keys;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.Documents
{
    public record DeletedChat
    {
        [BsonId]
        public required DeletedChatKey Id { get; init; }

        [BsonElement("deletedAt")]
        public required DateTime DeletedAt { get; init; }
    }
}
