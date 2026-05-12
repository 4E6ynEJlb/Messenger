namespace Application.Models.Output
{
    /// <summary>
    /// Information about user ban
    /// </summary>
    public record BanInformation
    {
        public required Guid UserId { get; init; }
        /// <summary>
        /// Administrator id
        /// </summary>
        public required int? BannedBy { get; init; }
        public required string Reason { get; init; }
        public required DateTime BannedAt { get; init; }
        public required DateTime UnbannedAt { get; init; }
    }
}
