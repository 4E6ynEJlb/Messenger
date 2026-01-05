namespace Application.Models.Input
{
    public record UserCredentials
    {
        public required byte[] Login { get; init; }
        public required byte[] Password { get; init; }
    }
}
