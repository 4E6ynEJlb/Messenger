using Application.Models.Input;
using UserAPI.Extensions;

namespace UserAPI.Models
{
    public record CreateBotForm
    {
        public required string BotName { get; init; }
        public required string Tag { get; init; }
        public string? BotDescription { get; init; }
        public IFormFile? BotAvatar { get; init; }
        public CreateBotModel ToCreateBotModel() => new CreateBotModel
        {
            BotName = BotName,
            Tag = Tag,
            BotDescription = BotDescription,
            BotAvatar = BotAvatar?.ToFileUpload()
        };
    }
}
