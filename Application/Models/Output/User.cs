using Application.Models.Input;
using Domain.Models.Types;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Output
{
    public record User : UpdateUser
    {
        public User() { }
        [SetsRequiredMembers]
        public User(UserData userData, string mediaPrefix)
        {
            UserId = userData.UserId;
            FirstName = userData.FirstName;
            LastName = userData.LastName;
            Tag = userData.Tag;
            Avatar = userData.Avatar is not null ? $"{mediaPrefix}/{userData.Avatar}" : null;
            BirthDate = userData.BirthDate;
            Bio = userData.Bio;
            WasOnline = userData.WasOnline;
        }
        public required string? Avatar { get; set; }
        public required Guid UserId { get; init; }
        public required DateTime WasOnline { get; init; }
    }
}
