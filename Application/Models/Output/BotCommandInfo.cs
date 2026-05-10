using Application.Models.Input;
using Application.Models.Internal;

namespace Application.Models.Output
{
    public record BotCommandInfo : AddCommandModel
    {
        public required uint Id { get; set; }
        public required CommandArgument[] Arguments { get; init; }
    }
}
