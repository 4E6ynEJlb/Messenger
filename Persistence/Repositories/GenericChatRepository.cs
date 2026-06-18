using Dapper;
using Domain.Models.Types;
using Domain.Stores;
using Npgsql;

namespace Persistence.Repositories
{
    public class GenericChatRepository : IGenericChatStore
    {
        private readonly UnitOfWorkConnectionScope _connectionScope;

        public GenericChatRepository(UnitOfWorkConnectionScope connectionScope)
        {
            _connectionScope = connectionScope;
        }

        public async Task<bool> CheckAccessToAttachmentAsync(EnChatType chatType, Guid chatId, Guid messageId,
            Guid attachmentId, CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken).ConfigureAwait(false);
                const string sql =
                    "SELECT sch_user.check_access_to_attachment(@chat_type, @chat_id, @message_id, @attachment_id)";
                return await lease.Connection.ExecuteScalarAsync<bool>(RepositoryExecution.Cmd(sql, new
                {
                    chat_type = chatType,
                    chat_id = chatId,
                    message_id = messageId,
                    attachment_id = attachmentId
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<bool> CheckAccessToMessageAsync(EnChatType chatType, Guid chatId, Guid userId, Guid messageId,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken).ConfigureAwait(false);
                const string sql =
                    "SELECT sch_user.check_access_to_message(@chat_type, @chat_id, @user_id, @message_id)";
                return await lease.Connection.ExecuteScalarAsync<bool>(RepositoryExecution.Cmd(sql, new
                {
                    chat_type = chatType,
                    chat_id = chatId,
                    user_id = userId,
                    message_id = messageId
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<Message[]> GetMessagesByIdAsync(Guid chatId, Guid[] messagesId, Guid gettingBy,
            EnChatType chatType, CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken).ConfigureAwait(false);
                const string sql =
                    "SELECT * FROM sch_user.get_messages_by_id(@chat_id, @message_ids, @getting_by, @chat_type)";
                var rows = await lease.Connection.QueryAsync<Message>(RepositoryExecution.Cmd(sql, new
                {
                    chat_id = chatId,
                    message_ids = messagesId,
                    getting_by = gettingBy,
                    chat_type = chatType
                }, cancellationToken)).ConfigureAwait(false);
                return rows.ToArray();
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task PrepareAttachmentsForResending(Guid chatId, Guid[] messagesId, Guid resendBy, EnChatType chatType, CancellationToken cancellationToken)
        {
            try
            {
                await using var lease = await _connectionScope.LeaseConnectionAsync(cancellationToken).ConfigureAwait(false);
                const string sql =
                    "CALL sch_user.prepare_attachments_for_resending(@chat_id, @chat_type, @messages, @resend_by)";
                await lease.Connection.ExecuteAsync(RepositoryExecution.Cmd(sql, new
                {
                    chat_id = chatId,
                    chat_type = chatType,
                    messages = messagesId,
                    resend_by = resendBy
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }
    }
}
