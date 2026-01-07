using Application.Models.Internal.Constants;

namespace Application.Models
{
    public record ReplyInfo
    {
        public required Guid ReplyTo { get; init; }
        public required ChatType ChatType { get; init; }
        public required Guid ChatId { get; init; }
    }
}
