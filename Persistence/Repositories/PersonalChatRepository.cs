using Dapper;
using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Database;
using Npgsql;
using Persistence.Exceptions;
using Persistence;

namespace Persistence.Repositories
{
    public class PersonalChatRepository : IPersonalChatStore
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PersonalChatRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task BlockUserAsync(Guid blockingBy, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.block_user(@blocking_by, @user_id)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { blocking_by = blockingBy, user_id = userId }, cancellationToken)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<Guid> CreateChatAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.initialize_new_personal_chat(@first_owner, @second_owner)";
                return await conn.ExecuteScalarAsync<Guid>(RepositoryExecution.Cmd(sql,
                    new { first_owner = userId1, second_owner = userId2 }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeleteChatAsync(Guid chatId, Guid deletingBy, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.delete_personal_chat(@chat_id, @deleting_by)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, deleting_by = deletingBy }, cancellationToken)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeleteFileFromMessageAsync(Guid chatId, Guid attachmentId, Guid deletingBy,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql =
                    "SELECT sch_user.delete_file_from_personal_message(@chat_id, @attachment_id, @deleting_by)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, attachment_id = attachmentId, deleting_by = deletingBy }, cancellationToken))
                    .ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid deletingBy, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.delete_personal_message(@chat_id, @message_id, @deleting_by)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, message_id = messageId, deleting_by = deletingBy }, cancellationToken))
                    .ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<ChatInformation> GetChatShortInfoAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_personal_chat_short_info(@chat_id, @user_id)";
                var row = await conn.QuerySingleOrDefaultAsync<ChatInformation>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, user_id = userId }, cancellationToken)).ConfigureAwait(false);
                return row ?? throw new ResourceNotFoundException(new Exception("Chat not found or access denied."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<Message> GetMessageAsync(Guid chatId, Guid messageId, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_personal_message(@chat_id, @message_id, @getting_by)";
                var row = await conn.QuerySingleOrDefaultAsync<Message>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, message_id = messageId, getting_by = userId }, cancellationToken))
                    .ConfigureAwait(false);
                return row ?? throw new ResourceNotFoundException(new Exception("Message not found."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<Guid> GetMessageIdByMediaAsync(Guid chatId, Guid mediaId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql =
                    "SELECT * FROM sch_user.get_message_id_by_media(@chat_id, @attachment_id, @chat_type)";
                return await conn.ExecuteScalarAsync<Guid>(RepositoryExecution.Cmd(sql, new
                {
                    chat_id = chatId,
                    attachment_id = mediaId,
                    chat_type = EnChatType.Personal
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<Message[]> GetMessagesAsync(Guid chatId, Guid gettingBy, uint messagesCount, DateTime sentBefore,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql =
                    "SELECT * FROM sch_user.get_messages_from_personal_chat(@chat_id, @getting_by, @messages_count, @sent_before)";
                var rows = await conn.QueryAsync<Message>(RepositoryExecution.Cmd(sql, new
                {
                    chat_id = chatId,
                    getting_by = gettingBy,
                    messages_count = (int)messagesCount,
                    sent_before = DateTime.SpecifyKind(sentBefore, DateTimeKind.Unspecified)
                }, cancellationToken)).ConfigureAwait(false);
                return rows.ToArray();
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<Guid> GetUserIdByChatIdAsync(Guid chatId, Guid gettingBy, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.get_personal_chat_destination_user_id(@chat_id, @getting_by)";
                return await conn.ExecuteScalarAsync<Guid>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, getting_by = gettingBy }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<Guid[]> ResendMessagesAsync(Guid chatId, Guid senderId, EnChatType sourceChatType, Guid sourceChatId,
            Guid[] messages, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = """
                    SELECT * FROM sch_user.resend_to_private_messages(
                        @chat_id,
                        @author,
                        @source_chat_type,
                        @source_chat_id,
                        @messages_id)
                    """;
                var rows = await conn.QueryAsync<Guid>(RepositoryExecution.Cmd(sql, new
                {
                    chat_id = chatId,
                    author = senderId,
                    source_chat_type = sourceChatType,
                    source_chat_id = sourceChatId,
                    messages_id = messages
                }, cancellationToken)).ConfigureAwait(false);
                return rows.ToArray();
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<Guid> SendMessageAsync(Guid chatId, Guid senderId, Guid? replyTo, string? text, MediaFile[]? attachments,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql =
                    "SELECT sch_user.send_personal_message(@chat_id, @author, @message_text, @attachments, @reply_to)";
                return await conn.ExecuteScalarAsync<Guid>(RepositoryExecution.Cmd(sql, new
                {
                    chat_id = chatId,
                    author = senderId,
                    message_text = text,
                    attachments,
                    reply_to = replyTo
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task UnblockUserAsync(Guid unblockingBy, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.unblock_user(@unblocking_by, @user_id)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { unblocking_by = unblockingBy, user_id = userId }, cancellationToken)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task UpdateMessageTextAsync(Guid chatId, Guid messageId, Guid senderId, string? newText,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql =
                    "SELECT sch_user.update_personal_message_text(@chat_id, @message_id, @author, @new_message_text)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    chat_id = chatId,
                    message_id = messageId,
                    author = senderId,
                    new_message_text = newText
                }, cancellationToken)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }
    }
}
