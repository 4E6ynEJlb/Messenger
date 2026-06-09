using Application.Models.Input;
using Application.Models.Internal;
using Domain.Models.Types;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Output
{
    /// <summary>
    /// Command info for bot command list
    /// </summary>
    public record BotCommandInfo : AddCommandModel
    {
        public BotCommandInfo() { }
        [SetsRequiredMembers]
        public BotCommandInfo(Domain.Models.Types.BotCommandInfo comm)
        {
            Id = comm.CommandId;
            Prefix = comm.Prefix;
            Command = comm.Command;
            Description = comm.Description;
            Arguments = comm.Arguments.Select(arg => new CommandArgument
            {
                Id = arg.ArgumentId,
                Name = arg.Name,
                Type = arg.Type
            }).ToArray();
        }
        public required uint Id { get; set; }
        /// <summary>
        /// List of arguments for current command
        /// </summary>
        public required CommandArgument[] Arguments { get; init; }
    }
}
