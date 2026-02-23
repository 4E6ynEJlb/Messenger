namespace Domain.Models
{
    public record UpdateUserAuthModel
    {
        public required Guid UserId { get; init; }
        public required string UserCurrentPassword { get; init; }
        public required string? UserNewLogin { get; init; }
        public required string? UserNewPassword { get; init; }
    }
}
