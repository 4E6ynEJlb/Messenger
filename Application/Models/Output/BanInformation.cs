namespace Application.Models.Output
{
    public record BanInformation
    {
        public required Guid UserId { get; init; }
        public required int BannedBy { get; init; }
        public required string Reason { get; init; }
        public required DateTime BannedAt { get; init; }
        public required DateTime UnbannedAt { get; init; }
    }
}
