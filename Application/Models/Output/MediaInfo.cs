namespace Application.Models.Output
{
    public record MediaInfo
    {
        public required string ContentType { get; init; }
        public required string FileName { get; init; }
    }
}
