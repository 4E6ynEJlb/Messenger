namespace Domain.Models.Types.Tables
{
    public record BannedUsers
    {
        public required Guid UserId { get; init; }
        public required int? BannedBy { get; init; }
        public required DateTime BannedAt { get; init; }
        public required DateTime UnbannedAt { get; init; }
        public required string Reason { get; init; }
    }
}
