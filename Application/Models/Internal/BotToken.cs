namespace Application.Models.Internal
{
    /// <summary>
    /// Information about bot access token
    /// </summary>
    public record BotToken
    {
        /// <summary>
        /// Auth token for bot, what should be copied by bot owner
        /// </summary>
        public required string Token { get; init; }
        /// <summary>
        /// Gets the version number of the token associated with this instance.
        /// </summary>
        public required uint TokenVersion { get; init; }
    }
}
