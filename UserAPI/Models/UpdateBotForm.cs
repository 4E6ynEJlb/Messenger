namespace UserAPI.Models
{
    public record UpdateBotForm
    {
        public required string? BotName { get; init; }
        public required string? Tag { get; init; }
        /// <summary>
        /// False if bot description should not be updated and true if it should be updated
        /// </summary>
        public required bool UpdateDescription { get; init; }
        public required string? BotDescription { get; init; }
        /// <summary>
        /// False if bot avatar should not be updated and true if it should be updated
        /// </summary>
        public required bool UpdateAvatar { get; init; }
        public required IFormFile? BotAvatar { get; init; }
    }
}
