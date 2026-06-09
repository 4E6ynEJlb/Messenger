using Domain.Models.Types;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Output
{
    /// <summary>
    /// Information about bot access token
    /// </summary>
    public record BotToken
    {
        public BotToken() { }
        [SetsRequiredMembers]
        public BotToken(BotTokenInfo token)
        {
            Token = Convert.ToBase64String(token.TokenHash);
            TokenVersion = token.TokenVersion;
        }
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
