using Domain.Models;

namespace Application.Models.Input
{
    /// <summary>
    /// Model for updating user credentials. For not updating fields send null
    /// </summary>
    public record UpdateCredentials : UserCredentials
    {
        public required string OldPassword { get; init; }
        internal UpdateUserAuthModel ToUpdateUserAuthModel(Guid id) => new UpdateUserAuthModel()
        {
            UserId = id,
            UserCurrentPassword = OldPassword,
            UserNewLogin = Login,
            UserNewPassword = Password
        };
    }
}
