namespace Application.Models.Input
{
    /// <summary>
    /// Model for updating message
    /// </summary>
    public record UpdatingMessage
    {
        public required Guid MessageId { get; init; }
        public required Guid ChatId { get; init; }
        /// <summary>
        /// Should be not null, if the message is text, and can be null if the message has files
        /// </summary>
        public required string? MessageText { get; init; }
    }
}
