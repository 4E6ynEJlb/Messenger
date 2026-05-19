using Domain.Models.Types;
using Domain.Models.Types.Tables;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Output
{
    /// <summary>
    /// Information about user ban
    /// </summary>
    public record BanInformation
    {
        public BanInformation() { }
        [SetsRequiredMembers]
        public BanInformation(BannedUsers banInfo)
        {
            UserId = banInfo.UserId;
            BannedBy = banInfo.BannedBy;
            Reason = banInfo.Reason;
            BannedAt = banInfo.BannedAt;
            UnbannedAt = banInfo.UnbannedAt;
        }
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
