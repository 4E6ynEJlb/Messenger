namespace Domain.Models.Types
{
    public record AttachmentInput
    {
        public required int? LinksCount { get; init; }
        public required Guid MediaId { get; init; }
        public required string? FileName { get; init; }
        public required string? ContentType { get; init; }
    }
}
