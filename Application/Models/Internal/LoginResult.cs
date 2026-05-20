namespace Application.Models.Internal
{
    public record LoginResult
    {
        public required Guid UserId { get; init; }
        public required Guid DeviceId { get; init; }
        public required uint ExpirationHours { get; init; }
        public required bool IsBanned { get; init; }
        public required string RefreshToken { get; init; }
    }
}
