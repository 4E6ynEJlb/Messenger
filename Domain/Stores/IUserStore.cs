using Domain.Models;
using Domain.Models.Types;
using Domain.Models.Types.Tables;

namespace Domain.Stores
{
    public interface IUserStore
    {
        public Task<UserData> GetUserByIdAsync(Guid userId);
        public Task<UserData> GetUserByTagAsync(string tag);
        public Task<UserData> AuthUserAsync(string login, string password);
        public Task<bool> CheckUserBanStatusAsync(Guid userId);
        public Task<BannedUsers> GetBannedUserInformationAsync(Guid userId);
        public Task<Guid[]> GetUserAvatarsAsync(Guid userId);
        public Task<ChatInformation[]> GetUserChatsAsync(Guid userId, uint page, uint pageSize);
        public Task<Guid> RegisterUserAsync(RegisterUserModel registerUserModel);
        public Task UpdateUserDataAsync(UserData newUserData);
        public Task UpdateUserAuthAsync(UpdateUserAuthModel updateUserAuthModel);
        public Task UploadUserAvatarAsync(Guid userId, MediaFile newUserAvatar);
        public Task DeleteUserAvatarAsync(Guid userId, Guid avatarId);
        public Task DeleteUserAsync(Guid userId, string userPassword);
    }
}
