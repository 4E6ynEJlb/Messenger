using Dapper;
using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Database;
using Npgsql;
using Persistence.Exceptions;
using Persistence;

namespace Persistence.Repositories
{
    public class BotChatRepository : IBotChatStore
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BotChatRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Guid> CreateChatAsync(Guid userId, Guid botId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.initialize_new_bot_chat(@user_id, @bot_id)";
                return await conn.ExecuteScalarAsync<Guid>(RepositoryExecution.Cmd(sql,
                    new { user_id = userId, bot_id = botId }, cancellationToken)).ConfigureAwait(false);
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
                const string sql = "SELECT sch_user.delete_bot_chat(@chat_id, @deleting_by)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, deleting_by = deletingBy }, cancellationToken)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DisableBotAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.disable_bot(@chat_id, @disabling_by)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, disabling_by = userId }, cancellationToken)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task EnableBotAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.enable_bot(@chat_id, @enabling_by)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, enabling_by = userId }, cancellationToken)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<BotButtonInfo[]> GetActiveButtonsListAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_active_buttons_list(@chat_id, @getting_by)";
                var rows = await conn.QueryAsync<BotButtonInfo>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, getting_by = userId }, cancellationToken)).ConfigureAwait(false);
                return rows.ToArray();
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<bool> GetBotChatAbilityAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.get_bot_chat_ability(@chat_id, @getting_by)";
                var v = await conn.ExecuteScalarAsync<bool?>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, getting_by = userId }, cancellationToken)).ConfigureAwait(false);
                return v ?? false;
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<Guid> GetBotIdByChatIdAsync(Guid chatId, Guid gettingBy, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.get_bot_id_by_chat_id(@chat_id, @getting_by)";
                return await conn.ExecuteScalarAsync<Guid>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, getting_by = gettingBy }, cancellationToken)).ConfigureAwait(false);
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
                const string sql = "SELECT * FROM sch_user.get_bot_chat_short_info(@chat_id, @getting_by)";
                return await conn.QuerySingleAsync<ChatInformation>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, getting_by = userId }, cancellationToken)).ConfigureAwait(false);
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
                const string sql = "SELECT * FROM sch_user.get_bot_message(@chat_id, @message_id, @getting_by)";
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
                    chat_type = EnChatType.Bot
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
                    "SELECT * FROM sch_user.get_bot_messages(@chat_id, @getting_by, @messages_count, @sent_before)";
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

        public async Task<Guid[]> ResendMessagesAsync(Guid chatId, Guid senderId, EnChatType sourceChatType, Guid sourceChatId,
            Guid[] messages, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = """
                    SELECT * FROM sch_user.resend_to_bot_messages(
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
                    "SELECT sch_user.send_bot_message(@chat_id, @author, @message_text, @attachments, @reply_to)";
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
    }
}
