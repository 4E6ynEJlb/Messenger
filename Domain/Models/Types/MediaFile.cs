namespace Domain.Models.Types
{
    public record MediaFile
    {
        public required Guid MediaId { get; init; }
        public required string FileName { get; init; }
        public required string ContentType { get; init; }
    }
}
