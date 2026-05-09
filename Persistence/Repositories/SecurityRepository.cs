using Dapper;
using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Database;
using Npgsql;
using Persistence;

namespace Persistence.Repositories
{
    public class SecurityRepository : ISecurityStore
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SecurityRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task ReportAdministratorAsync(Guid reportedBy, int adminId, string? comment, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.report_administrator(@reported_by, @administrator_id, @comment)";
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    reported_by = reportedBy,
                    administrator_id = adminId,
                    comment
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task ReportBotAsync(Guid reportedBy, Guid botId, string? comment, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.report_bot(@reported_by, @bot_id, @comment)";
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { reported_by = reportedBy, bot_id = botId, comment }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task ReportMessageAsync(Guid reportedBy, EnChatType chatType, Guid chatId, Guid messageId, string? comment,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql =
                    "SELECT sch_user.report_message(@reported_by, @chat_type, @chat_id, @message_id, @comment)";
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    reported_by = reportedBy,
                    chat_type = chatType,
                    chat_id = chatId,
                    message_id = messageId,
                    comment
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task ReportPublicChatAsync(Guid reportedBy, Guid chatId, string? comment, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.report_public_chat(@reported_by, @chat_id, @comment)";
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { reported_by = reportedBy, chat_id = chatId, comment }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task ReportUserAsync(Guid reportedBy, Guid reportedUserId, string? comment, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.report_user(@reported_by, @reported_user_id, @comment)";
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    reported_by = reportedBy,
                    reported_user_id = reportedUserId,
                    comment
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }
    }
}
