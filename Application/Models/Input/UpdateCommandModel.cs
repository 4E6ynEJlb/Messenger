namespace Application.Models.Input
{
    /// <summary>
    /// Model for updating bot command. For not updating fields send null, but for updating fields send new values.
    /// </summary>
    public record UpdateCommandModel
    {
        /// <summary>
        /// Updating command id
        /// </summary>
        public required uint Id { get; init; }
        /// <summary>
        /// Updating command prefix. Shoult not be letter or digit
        /// </summary>
        public required char? NewPrefix { get; init; }
        /// <summary>
        /// Max length is 8 chars, should start with a letter and contain letters and digits only
        /// </summary>
        public required string? NewCommand { get; init; }
        /// <summary>
        /// Command description for user tips. Max length is 32 chars
        /// </summary>
        public required string? NewDescription { get; init; }
    }
}
