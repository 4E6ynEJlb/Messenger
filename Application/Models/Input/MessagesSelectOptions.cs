namespace Application.Models.Input
{
    /// <summary>
    /// Model for getting messages from chat
    /// </summary>
    public record MessagesSelectOptions
    {
        /// <summary>
        /// Min value is 1. Number of messages to get. If messages count is greater 
        /// than total messages count in chat then all messages will be returned
        /// </summary>
        public required uint MessagesCount { get; init; }
        /// <summary>
        /// Send null if you want to get the most recent messages
        /// </summary>
        public required DateTime? SentBefore { get; init; }
    }
}
