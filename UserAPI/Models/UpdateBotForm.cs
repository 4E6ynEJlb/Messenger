namespace UserAPI.Models
{
    public record UpdateBotForm
    {
        public required string? BotName { get; init; }
        public required string? Tag { get; init; }
        public required bool UpdateDescription { get; init; }
        public required string? BotDescription { get; init; }
        public required bool UpdateAvatar { get; init; }
        public required IFormFile? BotAvatar { get; init; }
    }
}
