using Dapper;
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
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<UserData?> AuthUserAsync(string login, string password, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.auth_user(@login, @password)";
                return await conn.QuerySingleOrDefaultAsync<UserData>(
                    RepositoryExecution.Cmd(sql, new { login, password }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<bool> CheckUserBanStatusAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.check_user_ban_status(@user_id)";
                return await conn.ExecuteScalarAsync<bool>(
                    RepositoryExecution.Cmd(sql, new { user_id = userId }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeleteUserAsync(Guid userId, string userPassword, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.delete_user(@user_id, @user_password)";
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { user_id = userId, user_password = userPassword }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeleteUserAvatarAsync(Guid userId, Guid avatarId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.delete_user_avatar(@user_id, @avatar)";
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { user_id = userId, avatar = avatarId }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<BannedUsers?> GetBannedUserInformationAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_banned_user_information(@user_id)";
                return await conn.QuerySingleOrDefaultAsync<BannedUsers>(RepositoryExecution.Cmd(sql,
                    new { user_id = userId }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<Guid[]> GetUserAvatarsAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_user_avatars(@user_id)";
                var rows = await conn.QueryAsync<Guid>(
                    RepositoryExecution.Cmd(sql, new { user_id = userId }, cancellationToken)).ConfigureAwait(false);
                return rows.ToArray();
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<UserData?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_user_by_id(@user_id)";
                return await conn.QuerySingleOrDefaultAsync<UserData>(RepositoryExecution.Cmd(sql,
                    new { user_id = userId }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<UserData?> GetUserByTagAsync(string tag, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_user_by_tag(@tag)";
                return await conn.QuerySingleOrDefaultAsync<UserData>(RepositoryExecution.Cmd(sql,
                    new { tag }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<ChatInformation[]> GetUserChatsAsync(Guid userId, uint page, uint pageSize,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_user_chats(@user_id, @page, @page_size)";
                var rows = await conn.QueryAsync<ChatInformation>(RepositoryExecution.Cmd(sql,
                    new { user_id = userId, page = (int)page, page_size = (int)pageSize }, cancellationToken))
                    .ConfigureAwait(false);
                return rows.ToArray();
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<Guid> RegisterUserAsync(RegisterUserModel registerUserModel, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = """
                    SELECT sch_user.register_user(
                        @user_login::dom_auth_string,
                        @user_password::dom_auth_string,
                        @first_name,
                        @last_name,
                        @tag,
                        @birth_date)
                    """;
                var birth = registerUserModel.BirthDate.HasValue
                    ? registerUserModel.BirthDate.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null;
                return await conn.ExecuteScalarAsync<Guid>(RepositoryExecution.Cmd(sql, new
                {
                    user_login = registerUserModel.UserLogin,
                    user_password = registerUserModel.UserPassword,
                    first_name = registerUserModel.FirstName,
                    last_name = registerUserModel.LastName,
                    tag = registerUserModel.Tag,
                    birth_date = birth
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task UpdateUserAuthAsync(UpdateUserAuthModel updateUserAuthModel, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = """
                    SELECT sch_user.update_user_auth(
                        @user_id,
                        @user_current_password,
                        @user_new_login::dom_auth_string,
                        @user_new_password::dom_auth_string)
                    """;
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    user_id = updateUserAuthModel.UserId,
                    user_current_password = updateUserAuthModel.UserCurrentPassword,
                    user_new_login = updateUserAuthModel.UserNewLogin,
                    user_new_password = updateUserAuthModel.UserNewPassword
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task UpdateUserDataAsync(UserData newUserData, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = """
                    SELECT sch_user.update_user_data(
                        ROW(
                            @user_id,
                            @first_name,
                            @last_name,
                            @tag,
                            @avatar,
                            @birth_date,
                            @bio,
                            @was_online)::user_data)
                    """;
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    user_id = newUserData.UserId,
                    first_name = newUserData.FirstName,
                    last_name = newUserData.LastName,
                    tag = newUserData.Tag,
                    avatar = newUserData.Avatar,
                    birth_date = newUserData.BirthDate,
                    bio = newUserData.Bio,
                    was_online = newUserData.WasOnline
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task UploadUserAvatarAsync(Guid userId, MediaFile newUserAvatar, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = """
                    SELECT sch_user.update_user_avatar(
                        @user_id,
                        ROW(@media_id, @file_name, @content_type)::media_file)
                    """;
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    user_id = userId,
                    media_id = newUserAvatar.MediaId,
                    file_name = newUserAvatar.FileName,
                    content_type = newUserAvatar.ContentType
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }
    }
}
