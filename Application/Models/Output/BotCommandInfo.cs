using Application.Models.Input;
using Application.Models.Internal;

namespace Application.Models.Output
{
    /// <summary>
    /// Command info for bot command list
    /// </summary>
    public record BotCommandInfo : AddCommandModel
    {
        public required uint Id { get; set; }
        /// <summary>
        /// List of arguments for current command
        /// </summary>
        public required CommandArgument[] Arguments { get; init; }
    }
}
