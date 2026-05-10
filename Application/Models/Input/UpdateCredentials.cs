namespace Application.Models.Input
{
    public record UpdateCredentials : UserCredentials
    {
        public required string OldPassword { get; init; }
    }
}
