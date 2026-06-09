namespace Application.Models.Internal.Options
{
    public class ExpirationOptions
    {
        public const string OPTIONS_NAME = "ExpirationOptions";
        public required uint BotUserExpirationSeconds {  get; set; }
        public required uint MessagesExpirationSeconds { get; set; }
    }
}
