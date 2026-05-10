namespace Domain.Models.Types
{
    public record UserData
    {
        public required Guid UserId { get; init; }
        public required string FirstName { get; init; }
        public required string? LastName { get; init; }
        public required string Tag { get; init; }
        public required Guid? Avatar { get; init; }
        public required DateOnly? BirthDate { get; init; }
        public required string? Bio { get; init; }
        public required DateTime WasOnline { get; init; }
    }
}