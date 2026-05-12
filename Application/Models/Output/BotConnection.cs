using System.Net;

namespace Application.Models.Output
{
    /// <summary>
    /// Information about bot connection to the server
    /// </summary>
    public record BotConnection
    {
        /// <summary>
        /// IP address of the bot
        /// </summary>
        public required IPAddress IPAddress { get; init; }
        /// <summary>
        /// First connection time with current ip address to the server
        /// </summary>
        public required DateTime ConnectedAt { get; init; }
        /// <summary>
        /// Gets the version number of the token associated with this instance
        /// </summary>
        public required uint TokenVersion { get; init; }
    }
}
