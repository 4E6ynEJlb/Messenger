using Domain.Models.Documents.Keys;
using Domain.Models.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.Documents
{
    public record DeletedAttachment
    {
        [BsonId]
        public required DeletedAttachmentKey Id { get; init; }

        [BsonElement("deletedBy")]
        public required Guid DeletedBy { get; init; }

        [BsonElement("deletedAt")]
        public required DateTime DeletedAt { get; init; }
    }
}
