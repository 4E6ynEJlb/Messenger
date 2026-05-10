
namespace Domain.Models.Types
{
    public record BotButtonInfo
    {
        public required string ButtonText { get; init; }
        public required string InnerCommand { get; init; }
        public required byte[]? BackgroundColor { get; init; }
    }
}
