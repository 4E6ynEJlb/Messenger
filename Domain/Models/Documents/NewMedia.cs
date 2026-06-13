using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.Documents
{
    public record NewMedia
    {
        [BsonId]
        public required Guid MediaId { get; init; }

        [BsonElement("fileName")]
        public required string FileName { get; init; }

        [BsonElement("contentType")]
        public required string ContentType { get; init; }
    }
}
