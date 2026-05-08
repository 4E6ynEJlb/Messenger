using Application.Models.Internal;

namespace Application.Models.Input
{
    public record CreateBotModel
    {
        public required string BotName { get; init; }
        public required string Tag { get; init; }
        public required string? BotDescription { get; init; }
        public required FileUpload? BotAvatar { get; init; }
    }
}
