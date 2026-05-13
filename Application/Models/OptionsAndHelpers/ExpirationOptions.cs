namespace Application.Models.OptionsAndHelpers
{
    public class ExpirationOptions
    {
        public const string OPTIONS_NAME = "ExpirationOptions";
        public required uint BotUserExpirationSeconds {  get; set; }
        public required uint MessagesExpirationSeconds { get; set; }
    }
}
