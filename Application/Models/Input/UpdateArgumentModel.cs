namespace Application.Models.Input
{
    /// <summary>
    /// Model for updating bot command argument. For not updating fields send null, but for updating fields send new values.
    /// </summary>
    public record UpdateArgumentModel
    {
        /// <summary>
        /// Updating command id
        /// </summary>
        public required uint CommandId { get; init; }
        /// <summary>
        /// Updating argument id
        /// </summary>
        public required uint ArgumentId { get; init; }
        /// <summary>
        /// Updating argument name for user tips. Max length is 32 chars
        /// </summary>
        public required string? NewArgumentName { get; init; }
        /// <summary>
        /// Updating argument type for user tips. Max length is 32 chars
        /// </summary>
        public required string? NewArgumentType { get; init; }
    }
}
