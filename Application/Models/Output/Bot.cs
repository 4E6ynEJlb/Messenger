namespace Application.Models.Output
{
    /// <summary>
    /// Bot information model
    /// </summary>
    public record Bot
    {
        public required Guid BotId { get; init; }
        public required string Name { get; init; }
        public required string Tag { get; init; }
        public required string? Avatar { get; init; }
        public required string? Description { get; init; }
        /// <summary>
        /// Administrators can enable and disable bots. 
        /// If the bot is disabled, it cannot send messages and interact with users, 
        /// but it can still receive messages and interact with users. 
        /// This allows administrators to temporarily disable a bot without deleting it.
        /// </summary>
        public required bool IsEnabled { get; init; }
    }
}
