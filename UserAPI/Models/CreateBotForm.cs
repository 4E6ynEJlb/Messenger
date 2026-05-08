namespace UserAPI.Models
{
    public record CreateBotForm
    {
        public required string BotName { get; init; }
        public required string Tag { get; init; }
        public required string? BotDescription { get; init; }
        public required IFormFile? BotAvatar { get; init; }
    }
}
