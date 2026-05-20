namespace Application.Models.Internal
{
    public record TokenValidationResult
    {
        public required string Token { get; init; }
        public required bool IsBanned { get; init; }
    }
}
