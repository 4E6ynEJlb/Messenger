namespace Application.Models.Output
{
    public record Color
    {
        public required byte R { get; init; }
        public required byte G { get; init; }
        public required byte B { get; init; }
    }
}
