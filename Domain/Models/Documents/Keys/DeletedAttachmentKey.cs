using Domain.Models.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Models.Documents.Keys
{
    public record DeletedAttachmentKey
    {
        [BsonElement("mediaId")]
        public required Guid MediaId { get; init; }

        [BsonElement("messageId")]
        public required Guid MessageId { get; init; }

        [BsonElement("chatId")]
        public required Guid ChatId { get; init; }

        [BsonElement("chatType")]
        [BsonRepresentation(BsonType.String)]
        public required EnChatType ChatType { get; init; }
    }
}
