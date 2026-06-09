namespace Application.Models.Input
{
    /// <summary>
    /// Model for adding argument to bot command. All fields are required. Max length for string fields is 32 chars.
    /// </summary>
    public record AddArgumentModel
    {
        public required uint CommandId { get; init; }
        /// <summary>
        /// Must appear in tip for user
        /// </summary>
        public required string ArgumentName { get; init; }
        /// <summary>
        /// Must appear in tip for user
        /// </summary>
        public required string ArgumentType { get; init; }
    }
}
