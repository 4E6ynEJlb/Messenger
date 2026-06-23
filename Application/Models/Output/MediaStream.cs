namespace Application.Models.Output
{
    public record MediaStream : MediaInfo
    {
        public required Stream Content { get; init; }
    }
}
