namespace Application.Models.Input
{
    public record UserCredentials
    {
        public required string Login { get; init; }
        public required string Password { get; init; }
    }
}
