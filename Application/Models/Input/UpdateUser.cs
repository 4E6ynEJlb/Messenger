namespace Application.Models.Input
{
    public record UpdateUser
    {
        public required string FirstName { get; init; }
        public required string? LastName { get; init; }
        public required string Tag { get; init; }
        public required DateOnly? BirthDate { get; init; }
        public required string? Bio { get; init; }
    }
}
