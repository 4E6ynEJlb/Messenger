namespace Application.Models.Internal
{
    /// <summary>
    /// Should be used as a tip for bot command
    /// </summary>
    public record CommandArgument
    {
        /// <summary>
        /// Should be used for inner sorting on client side
        /// </summary>
        public required uint Id { get; init; }
        /// <summary>
        /// Updating argument name for user tips. Max length is 32 chars
        /// </summary>
        public required string Name { get; init; }
        /// <summary>
        /// Updating argument type for user tips. Max length is 32 chars
        /// </summary>
        public required string Type { get; init; }
    }
}
