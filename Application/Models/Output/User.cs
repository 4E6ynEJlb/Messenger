using Application.Models.Input;

namespace Application.Models.Output
{
    public record User : UpdateUser
    {
        public required string? Avatar { get; set; }
        public required Guid UserId { get; init; }
        public required DateTime WasOnline { get; init; }
    }
}
