using Domain.Models;
using Domain.Models.Types;
using Domain.Models.Types.Tables;

namespace Domain.Stores
{
    public interface IUserStore
    {
        public Task<UserData?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
        public Task<UserData?> GetUserByTagAsync(string tag, CancellationToken cancellationToken);
        public Task<UserData?> AuthUserAsync(string login, string password, CancellationToken cancellationToken);
        public Task<bool> CheckUserBanStatusAsync(Guid userId, CancellationToken cancellationToken);
        public Task<BannedUsers?> GetBannedUserInformationAsync(Guid userId, CancellationToken cancellationToken);
        public Task<Guid[]> GetUserAvatarsAsync(Guid userId, CancellationToken cancellationToken);
        public Task<ChatInformation[]> GetUserChatsAsync(Guid userId, uint page, uint pageSize, CancellationToken cancellationToken);
        public Task<Guid> RegisterUserAsync(RegisterUserModel registerUserModel, CancellationToken cancellationToken);
        public Task UpdateUserDataAsync(UserData newUserData, CancellationToken cancellationToken);
        public Task UpdateUserAuthAsync(UpdateUserAuthModel updateUserAuthModel, CancellationToken cancellationToken);
        public Task UploadUserAvatarAsync(Guid userId, MediaFile newUserAvatar, CancellationToken cancellationToken);
        public Task DeleteUserAvatarAsync(Guid userId, Guid avatarId, CancellationToken cancellationToken);
        public Task DeleteUserAsync(Guid userId, string userPassword, CancellationToken cancellationToken);
    }
}
