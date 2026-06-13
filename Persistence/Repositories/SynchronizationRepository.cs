using Dapper;
using Domain.Models.Types;
using Domain.Stores;
using Npgsql;
using Persistence.Exceptions;

namespace Persistence.Repositories
{
    public class SynchronizationRepository : ISynchronizationStore
    {
        private readonly UnitOfWorkConnectionScope _connectionScope;

        public SynchronizationRepository(UnitOfWorkConnectionScope connectionScope)
        {
            _connectionScope = connectionScope;
        }

        public async Task DeleteFileFromPersonalMessageAsync(Guid chatId, Guid attachmentId, Guid deletingBy,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken)
                    .ConfigureAwait(false);
                const string sql =
                    "SELECT private.delete_file_from_personal_message(@chat_id, @attachment_id, @deleting_by)";
                var affected = await lease.Connection.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, attachment_id = attachmentId, deleting_by = deletingBy },
                    cancellationToken, lease.Transaction)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeletePersonalMessageAsync(Guid chatId, Guid messageId, Guid deletingBy,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken)
                    .ConfigureAwait(false);
                const string sql = "SELECT private.delete_personal_message(@chat_id, @message_id, @deleting_by)";
                var affected = await lease.Connection.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, message_id = messageId, deleting_by = deletingBy },
                    cancellationToken, lease.Transaction)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeleteFileFromPublicMessageAsync(Guid chatId, Guid attachmentId, Guid deletingBy,
            DateTime deletedAt, CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken)
                    .ConfigureAwait(false);
                const string sql =
                    "SELECT private.delete_file_from_public_message(@chat_id, @attachment_id, @deleting_by, @deleted_at)";
                var affected = await lease.Connection.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    chat_id = chatId,
                    attachment_id = attachmentId,
                    deleting_by = deletingBy,
                    deleted_at = deletedAt,
                }, cancellationToken, lease.Transaction)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeletePublicMessageAsync(Guid chatId, Guid messageId, Guid deletingBy,
            DateTime deletedAt, CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken)
                    .ConfigureAwait(false);
                const string sql = "SELECT private.delete_public_message(@chat_id, @message_id, @deleting_by, @deleted_at)";
                var affected = await lease.Connection.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new 
                { 
                    chat_id = chatId, 
                    message_id = messageId, 
                    deleting_by = deletingBy, 
                    deleted_at = deletedAt 
                },
                    cancellationToken, lease.Transaction)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task SaveMessageAsync(EnChatType chatType, Guid chatId, MessageInput message,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken)
                    .ConfigureAwait(false);
                const string sql = """
                    SELECT private.save_message(
                        @chat_type,
                        @chat_id,
                        @message::message_input)
                    """;
                var affected = await lease.Connection.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    chat_type = chatType,
                    chat_id = chatId,
                    message
                }, cancellationToken, lease.Transaction)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<int> UpdateMessageAsync(EnChatType chatType, Guid chatId, Guid messageId, string? newMessageText,
            DateTime updatedAt, CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken)
                    .ConfigureAwait(false);
                const string sql = """
                    SELECT private.update_message_text(
                        @chat_type,
                        @chat_id,
                        @message_id,
                        @new_message_text,
                        @updated_at)
                    """;
                return await lease.Connection.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    chat_type = chatType,
                    chat_id = chatId,
                    message_id = messageId,
                    new_message_text = newMessageText,
                    updated_at = DateTime.SpecifyKind(updatedAt, DateTimeKind.Unspecified)
                }, cancellationToken, lease.Transaction)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task LogPublicMessageUpdateAsync(Guid chatId, Guid updatedBy, DateTime updatedAt, 
            CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken)
                    .ConfigureAwait(false);
                const string sql = "SELECT private.log_public_message_update(@chat_id, @updated_by, @updated_at)";
                var affected = await lease.Connection.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    chat_id = chatId,
                    updated_by = updatedBy,
                    updated_at = DateTime.SpecifyKind(updatedAt, DateTimeKind.Unspecified)
                }, cancellationToken, lease.Transaction)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }
    }
}
