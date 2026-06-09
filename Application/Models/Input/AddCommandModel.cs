namespace Application.Models.Input
{
    /// <summary>
    /// Model for adding new command to the bot. 
    /// Command is a string that starts with a prefix and is used for user tips in the bot chat.
    /// </summary>
    public record AddCommandModel
    {
        /// <summary>
        /// Updating command prefix. Shoult not be letter or digit
        /// </summary>
        public required char Prefix { get; init; }
        /// <summary>
        /// Max length is 8 chars, should start with a letter and contain letters and digits only
        /// </summary>
        public required string Command { get; init; }
        /// <summary>
        /// Command description for user tips. Max length is 32 chars
        /// </summary>
        public required string Description { get; init; }
    }
}
