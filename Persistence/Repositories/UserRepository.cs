using Domain.Models;
using Domain.Models.Types;
using Domain.Models.Types.Tables;
using Domain.Stores;
using Infrastructure.Database;
using Npgsql;

namespace Persistence.Repositories
{
    public class UserRepository : IUserStore
    {
        private readonly IDbConnectionFactory  _connectionFactory;
        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<UserData> AuthUserAsync(string login, string password, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckUserBanStatusAsync(Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(Guid userId, string userPassword, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAvatarAsync(Guid userId, Guid avatarId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<BannedUsers> GetBannedUserInformationAsync(Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid[]> GetUserAvatarsAsync(Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserData> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserData> GetUserByTagAsync(string tag, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ChatInformation[]> GetUserChatsAsync(Guid userId, uint page, uint pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> RegisterUserAsync(RegisterUserModel registerUserModel, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserAuthAsync(UpdateUserAuthModel updateUserAuthModel, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserDataAsync(UserData newUserData, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UploadUserAvatarAsync(Guid userId, MediaFile newUserAvatar, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
