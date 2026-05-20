using Domain.Models.Types;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Output
{
    /// <summary>
    /// Bot information model
    /// </summary>
    public record Bot
    {
        public Bot() { }
        [SetsRequiredMembers]
        public Bot(BotInfo botInfo, string mediaPrefix)
        {
            BotId = botInfo.BotId;
            Name = botInfo.Name;
            Tag = botInfo.Tag;
            Avatar = $"{mediaPrefix}/{botInfo.Avatar}";
            Description = botInfo.Description;
            IsEnabled = botInfo.IsEnabled;
        }
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
