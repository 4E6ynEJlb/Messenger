namespace Application.Models.Output
{
    public record BotButton
    {
        public required string Text { get; init; }
        public required string Command { get; init; }
        public required Color? BackgroundColor { get; init; }
    }
}
