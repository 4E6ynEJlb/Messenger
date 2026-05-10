namespace Domain.Models
{
    public record RegisterUserModel
    {
        public required string UserLogin { get; init; }
        public required string UserPassword { get; init; }
        public required string FirstName { get; init; }
        public string? LastName { get; init; }
        public required string Tag { get; init; }
        public required DateOnly? BirthDate { get; init; } ///////rmk
    }
}
