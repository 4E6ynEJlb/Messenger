using Dapper;
using Domain.Stores;
using Infrastructure.Database;
using Npgsql;
using Persistence;

namespace Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenStore
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RefreshTokenRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<string> CreateRefreshTokenAsync(Guid userId, TimeSpan lifetime, Guid deviceId,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.create_refresh_token(@user_id, @lifetime, @device_id)";
                var token = await conn.ExecuteScalarAsync<string>(RepositoryExecution.Cmd(sql, new
                {
                    user_id = userId,
                    lifetime,
                    device_id = deviceId
                }, cancellationToken)).ConfigureAwait(false);
                return token ?? throw new InvalidOperationException("Не удалось создать refresh-токен.");
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.invalidate_refresh_token(@token)";
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new { token = refreshToken }, cancellationToken))
                    .ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<string> UpdateRefreshTokenAsync(Guid userId, string oldToken, Guid deviceId,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.update_refresh_token(@user_id, @old_token, @device_id)";
                var result = await conn.ExecuteScalarAsync<string>(RepositoryExecution.Cmd(sql, new
                {
                    user_id = userId,
                    old_token = oldToken,
                    device_id = deviceId
                }, cancellationToken)).ConfigureAwait(false);
                return result ?? throw new InvalidOperationException("Не удалось обновить refresh-токен.");
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken, Guid deviceId, Guid userId,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.validate_refresh_token(@token, @device_id, @user_id)";
                return await conn.ExecuteScalarAsync<bool>(RepositoryExecution.Cmd(sql, new
                {
                    token = refreshToken,
                    device_id = deviceId,
                    user_id = userId
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }
    }
}
