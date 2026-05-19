namespace Application.Models.Internal.Options
{
    public class TokenExpirationOptions
    {
        public const string OPTIONS_NAME = "TokenExpirationOptions";
        public required uint RememberingHours { get; set; }
        public required uint EphemeralHours { get; set; }
    }
}
