using Dapper;
using Domain.Stores;
using Npgsql;

namespace Persistence.Repositories
{
    public class MaintenanceRepository : IMaintenanceStore
    {
        private readonly UnitOfWorkConnectionScope _connectionScope;

        public MaintenanceRepository(UnitOfWorkConnectionScope connectionScope)
        {
            _connectionScope = connectionScope;
        }

        public async Task<Guid[]> ClearDeletedMediaAsync(CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken)
                    .ConfigureAwait(false);
                const string sql = "SELECT * FROM private.clear_deleted_media()";
                var rows = await lease.Connection.QueryAsync<Guid>(
                        RepositoryExecution.Cmd(sql, null, cancellationToken, lease.Transaction))
                    .ConfigureAwait(false);
                return rows.ToArray();
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<int> DeleteDesolatedChatsAsync(CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken)
                    .ConfigureAwait(false);
                const string sql = "SELECT private.delete_desolate_chats()";
                return await lease.Connection.ExecuteScalarAsync<int>(
                        RepositoryExecution.Cmd(sql, null, cancellationToken, lease.Transaction))
                    .ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task UnbanUsersBySentenceTimeAsync(CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken)
                    .ConfigureAwait(false);
                const string sql = "CALL private.unban_users_by_sentence_time()";
                await lease.Connection.ExecuteAsync(
                        RepositoryExecution.Cmd(sql, null, cancellationToken, lease.Transaction))
                    .ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeleteExpireddRefreshTokensAsync(CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken)
                    .ConfigureAwait(false);
                const string sql = "CALL private.delete_expired_refresh_tokens()";
                await lease.Connection.ExecuteAsync(
                        RepositoryExecution.Cmd(sql, null, cancellationToken, lease.Transaction))
                    .ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task CreateLogTableForCurrentMonthAsync(CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken)
                    .ConfigureAwait(false);
                const string sql = "CALL private.create_log_table_for_current_month()";
                await lease.Connection.ExecuteAsync(
                        RepositoryExecution.Cmd(sql, null, cancellationToken, lease.Transaction))
                    .ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeleteLogTablesAsync(int expirationMonths, CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken)
                    .ConfigureAwait(false);
                const string sql = "CALL private.delete_log_tables(@expiration_months)";
                await lease.Connection.ExecuteAsync(RepositoryExecution.Cmd(sql, new { expiration_months = expirationMonths },
                    cancellationToken, lease.Transaction)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }
    }
}
