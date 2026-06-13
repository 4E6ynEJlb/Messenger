using Dapper;
using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Database;
using Npgsql;
using Persistence.Exceptions;

namespace Persistence.Repositories
{
    public class PublicChatRepository : IPublicChatStore
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PublicChatRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<AuditLogRecord[]> AuditChatAsync(Guid chatId, Guid gettingBy, uint pageNumber, uint pageSize,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql =
                    "SELECT * FROM sch_user.audit_chat(@chat_id, @getting_by, @page_number, @page_size)";
                var rows = await conn.QueryAsync<AuditLogRecord>(RepositoryExecution.Cmd(sql, new
                {
                    chat_id = chatId,
                    getting_by = gettingBy,
                    page_number = (int)pageNumber,
                    page_size = (int)pageSize
                }, cancellationToken)).ConfigureAwait(false);
                return rows.ToArray();
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task BanUserAsync(Guid chatId, Guid userId, Guid banningBy, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.ban_user(@chat_id, @user_id, @banning_by)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, user_id = userId, banning_by = banningBy }, cancellationToken))
                    .ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<Guid> CreateNewChatAsync(string chatName, Guid creatorId, bool isSearchable, MediaFile? avatar,
            EnPublicChatMemberRole defaultMemberRole, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = """
                    SELECT sch_user.initialize_new_public_chat(
                        @chat_name::dom_public_chat_name,
                        @creator_id,
                        @is_searchable,
                        ROW(@media_id, @file_name, @content_type)::media_file,
                        @default_member_role::en_public_chat_member_role)
                    """;
                var result = await conn.ExecuteScalarAsync<Guid>(RepositoryExecution.Cmd(sql, new
                {
                    chat_name = chatName,
                    creator_id = creatorId,
                    is_searchable = isSearchable,
                    media_id = avatar?.MediaId,
                    file_name = avatar?.FileName,
                    content_type = avatar?.ContentType,
                    default_member_role = defaultMemberRole.ToString()
                }, cancellationToken)).ConfigureAwait(false);
                if (result == Guid.Empty) throw new DatabaseUpdateException(new Exception("No rows affected."));
                return result;
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<bool> CheckMessageSendingAbilityAsync(Guid chatId, Guid senderId, Guid? replyTo,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql =
                    "SELECT sch_user.check_public_message_send_ability(@chat_id, @author, @reply_to)";
                return await conn.ExecuteScalarAsync<bool>(RepositoryExecution.Cmd(sql, new
                {
                    chat_id = chatId,
                    author = senderId,
                    reply_to = replyTo
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeleteAndBanChatMemberAsync(Guid chatId, Guid memberId, Guid deletingBy,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.delete_and_ban_chat_member(@chat_id, @user_id, @deleting_by)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, user_id = memberId, deleting_by = deletingBy }, cancellationToken))
                    .ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
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
                const string sql = "SELECT sch_user.delete_public_chat(@chat_id, @deleting_by)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, deleting_by = deletingBy }, cancellationToken)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeleteChatMemberAsync(Guid chatId, Guid memberId, Guid deletingBy, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.delete_member_from_public_chat(@chat_id, @deleting_by, @deleting_user)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    chat_id = chatId,
                    deleting_by = deletingBy,
                    deleting_user = memberId
                }, cancellationToken)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }
        
        public async Task<PublicChatBannedUser[]> GetBannedUsersAsync(Guid chatId, Guid gettingBy, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_banned_users(@chat_id, @getting_by)";
                var rows = await conn.QueryAsync<PublicChatBannedUser>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, getting_by = gettingBy }, cancellationToken)).ConfigureAwait(false);
                return rows.ToArray();
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<PublicChatFullInformation> GetChatFullInfoAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_public_chat_full_information(@chat_id, @getting_by)";
                return await conn.QuerySingleAsync<PublicChatFullInformation>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, getting_by = userId }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<PublicChatOptions> GetChatOptionsAsync(Guid chatId, Guid gettingBy, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_public_chat_options(@chat_id, @getting_by)";
                return await conn.QuerySingleAsync<PublicChatOptions>(RepositoryExecution.Cmd(sql,
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
                const string sql = "SELECT * FROM sch_user.get_public_chat_short_info(@chat_id, @getting_by)";
                return await conn.QuerySingleAsync<ChatInformation>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, getting_by = userId }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<Message> GetMessageAsync(Guid chatId, Guid messageId, Guid gettingBy, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_public_message(@chat_id, @message_id, @getting_by)";
                var row = await conn.QuerySingleOrDefaultAsync<Message>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, message_id = messageId, getting_by = gettingBy }, cancellationToken))
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
                    chat_type = EnChatType.Public
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
                    "SELECT * FROM sch_user.get_messages_from_public_chat(@chat_id, @getting_by, @messages_count, @sent_before)";
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

        public async Task GiveMemberRoleAsync(Guid member, Guid chatId, Guid givingBy, EnPublicChatMemberRole newRole,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql =
                    "SELECT sch_user.give_public_chat_member_role(@chat_id, @member_id, @role::en_public_chat_member_role, @giving_by)";
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    chat_id = chatId,
                    member_id = member,
                    role = newRole.ToString(),
                    giving_by = givingBy
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task JoinChatAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.join_chat(@chat_id, @user_id)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, user_id = userId }, cancellationToken)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task LeaveChatAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.leave_chat(@chat_id, @user_id)";
                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, user_id = userId }, cancellationToken)).ConfigureAwait(false);
                if (affected == 0) throw new DatabaseUpdateException(new Exception("No rows affected."));
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }        

        public async Task<PublicChatFullInformation[]> SearchChatsAsync(string namePart, Guid gettingBy, uint pageNumber,
            uint pageSize, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql =
                    "SELECT * FROM sch_user.search_chats(@name_part, @getting_by, @page_number, @page_size)";
                var rows = await conn.QueryAsync<PublicChatFullInformation>(RepositoryExecution.Cmd(sql, new
                {
                    name_part = namePart,
                    getting_by = gettingBy,
                    page_number = (int)pageNumber,
                    page_size = (int)pageSize
                }, cancellationToken)).ConfigureAwait(false);
                return rows.ToArray();
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }
        
        public async Task UnbanUserAsync(Guid chatId, Guid userId, Guid unbanningBy, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.unban_user(@chat_id, @user_id, @unbanning_by)";
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { chat_id = chatId, user_id = userId, unbanning_by = unbanningBy }, cancellationToken))
                    .ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task UpdateChatAsync(Guid chatId, Guid updatingBy, string? newName, bool? isSearchable, bool updateAvatar,
            MediaFile? newAvatar, EnPublicChatMemberRole? defaultMemberRole, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                string sql;
                object param;
                if (newAvatar is null)
                {
                    sql = """
                        SELECT sch_user.update_public_chat(
                            @chat_id,
                            @updating_by,
                            @update_avatar,
                            @chat_name,
                            @is_searchable,
                            NULL::media_file,
                            @default_member_role::en_public_chat_member_role)
                        """;
                    param = new
                    {
                        chat_id = chatId,
                        updating_by = updatingBy,
                        update_avatar = updateAvatar,
                        chat_name = newName,
                        is_searchable = isSearchable,
                        default_member_role = defaultMemberRole.ToString()
                    };
                }
                else
                {
                    sql = """
                        SELECT sch_user.update_public_chat(
                            @chat_id,
                            @updating_by,
                            @update_avatar,
                            @chat_name,
                            @is_searchable,
                            ROW(@media_id, @file_name, @content_type)::media_file,
                            @default_member_role::en_public_chat_member_role)
                        """;
                    param = new
                    {
                        chat_id = chatId,
                        updating_by = updatingBy,
                        update_avatar = updateAvatar,
                        chat_name = newName,
                        is_searchable = isSearchable,
                        media_id = newAvatar.MediaId,
                        file_name = newAvatar.FileName,
                        content_type = newAvatar.ContentType,
                        default_member_role = defaultMemberRole.ToString()
                    };
                }

                var affected = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, param, cancellationToken)).ConfigureAwait(false);
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
                    "SELECT sch_user.update_public_text_message(@chat_id, @message_id, @author, @new_message_text)";
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    chat_id = chatId,
                    message_id = messageId,
                    author = senderId,
                    new_message_text = newText
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }
    }
}
