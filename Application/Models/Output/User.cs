using Application.Models.Input;

namespace Application.Models.Output
{
    public record User : UpdateUser
    {
        public required Guid UserId { get; init; }
        public required DateTime WasOnline { get; init; }
        public required bool IsBanned { get; init; }
    }
}
