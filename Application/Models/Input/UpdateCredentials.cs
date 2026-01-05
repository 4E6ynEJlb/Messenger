namespace Application.Models.Input
{
    public record UpdateCredentials : UserCredentials
    {
        public required byte[] OldPassword { get; init; }
    }
}
