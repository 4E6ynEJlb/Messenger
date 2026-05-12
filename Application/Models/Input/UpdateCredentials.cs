namespace Application.Models.Input
{
    /// <summary>
    /// Model for updating user credentials. For not updating fields send null
    /// </summary>
    public record UpdateCredentials : UserCredentials
    {
        public required string OldPassword { get; init; }
    }
}
