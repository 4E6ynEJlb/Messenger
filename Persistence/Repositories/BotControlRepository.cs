using Dapper;
using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Database;
using Npgsql;
using Persistence;

namespace Persistence.Repositories
{
    public class BotControlRepository : IBotControlStore
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BotControlRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<uint> AddCommandArgumentAsync(Guid botId, Guid addingBy, uint commandId, string name, string type,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql =
                    "SELECT sch_user.add_command_argument(@bot_id, @adding_by, @command_id, @name, @type)";
                var v = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    bot_id = botId,
                    adding_by = addingBy,
                    command_id = (int)commandId,
                    name,
                    type
                }, cancellationToken)).ConfigureAwait(false);
                return (uint)v;
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<uint> AddCommandAsync(Guid botId, Guid addingBy, char prefix, string command, string commandDescription,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql =
                    "SELECT sch_user.add_command(@bot_id, @adding_by, @prefix, @command, @description)";
                var v = await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    bot_id = botId,
                    adding_by = addingBy,
                    prefix,
                    command,
                    description = commandDescription
                }, cancellationToken)).ConfigureAwait(false);
                return (uint)v;
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<Guid> CreateBotAsync(string botName, string tag, string? botDescription, Guid owner, MediaFile? botAvatar,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                string sql;
                object param;
                if (botAvatar is null)
                {
                    sql = "SELECT sch_user.create_bot(@name, @tag, @owner, NULL::media_file, @description)";
                    param = new { name = botName, tag, owner, description = botDescription };
                }
                else
                {
                    sql = """
                        SELECT sch_user.create_bot(
                            @name,
                            @tag,
                            @owner,
                            ROW(@media_id, @file_name, @content_type)::media_file,
                            @description)
                        """;
                    param = new
                    {
                        name = botName,
                        tag,
                        owner,
                        media_id = botAvatar.MediaId,
                        file_name = botAvatar.FileName,
                        content_type = botAvatar.ContentType,
                        description = botDescription
                    };
                }

                return await conn.ExecuteScalarAsync<Guid>(RepositoryExecution.Cmd(sql, param, cancellationToken))
                    .ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeleteBotAsync(Guid botId, Guid deletingBy, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.delete_bot(@bot_id, @deleting_by)";
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql,
                    new { bot_id = botId, deleting_by = deletingBy }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeleteCommandArgumentAsync(Guid botId, Guid deletingBy, uint commandId, uint argumentId,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql =
                    "SELECT sch_user.delete_command_argument(@bot_id, @deleting_by, @command_id, @argument_id)";
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    bot_id = botId,
                    deleting_by = deletingBy,
                    command_id = (int)commandId,
                    argument_id = (int)argumentId
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task DeleteCommandAsync(Guid botId, Guid deletingBy, uint commandId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_user.delete_command(@bot_id, @deleting_by, @command_id)";
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    bot_id = botId,
                    deleting_by = deletingBy,
                    command_id = (int)commandId
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<BotInfo> GetBotByNameAsync(string botName, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_bot_by_name(@name)";
                return await conn.QuerySingleAsync<BotInfo>(RepositoryExecution.Cmd(sql, new { name = botName }, cancellationToken))
                    .ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<BotInfo> GetBotByTagAsync(string tag, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_bot_by_tag(@tag)";
                return await conn.QuerySingleAsync<BotInfo>(RepositoryExecution.Cmd(sql, new { tag }, cancellationToken))
                    .ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<BotInfo> GetBotInfoAsync(Guid botId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_bot_info(@bot_id)";
                return await conn.QuerySingleAsync<BotInfo>(RepositoryExecution.Cmd(sql, new { bot_id = botId }, cancellationToken))
                    .ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<BotTokenInfo> GetBotTokenAsync(Guid botId, Guid gettingBy, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.get_bot_token(@bot_id, @getting_by)";
                return await conn.QuerySingleAsync<BotTokenInfo>(RepositoryExecution.Cmd(sql,
                    new { bot_id = botId, getting_by = gettingBy }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<BotCommandInfo[]> ListBotCommandsAsync(Guid botId, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.list_commands(@bot_id)";
                var rows = await conn.QueryAsync<BotCommandInfo>(RepositoryExecution.Cmd(sql, new { bot_id = botId }, cancellationToken))
                    .ConfigureAwait(false);
                return rows.ToArray();
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<BotConnectionLogRecord[]> ListBotConnectionsAsync(Guid botId, Guid gettingBy,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.list_bot_connections(@bot_id, @getting_by)";
                var rows = await conn.QueryAsync<BotConnectionLogRecord>(RepositoryExecution.Cmd(sql,
                    new { bot_id = botId, getting_by = gettingBy }, cancellationToken)).ConfigureAwait(false);
                return rows.ToArray();
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<BotInfo[]> ListPersonalBotsAsync(Guid gettingBy, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.list_bots(@getting_by)";
                var rows = await conn.QueryAsync<BotInfo>(RepositoryExecution.Cmd(sql, new { getting_by = gettingBy }, cancellationToken))
                    .ConfigureAwait(false);
                return rows.ToArray();
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task<BotTokenInfo> RegenerateBotTokenAsync(Guid botId, Guid updatingBy, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT * FROM sch_user.regenerate_bot_token(@bot_id, @updating_by)";
                return await conn.QuerySingleAsync<BotTokenInfo>(RepositoryExecution.Cmd(sql,
                    new { bot_id = botId, updating_by = updatingBy }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task UpdateBotAsync(Guid botId, Guid updatingBy, bool updateDescription, bool updateAvatar, string? newName,
            string? newTag, string? newDescription, MediaFile? newAvatar, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                string sql;
                object param;
                if (newAvatar is null)
                {
                    sql = """
                        SELECT sch_user.update_bot(
                            @bot_id,
                            @updating_by,
                            @update_avatar,
                            @update_description,
                            @name,
                            @tag,
                            NULL::media_file,
                            @description)
                        """;
                    param = new
                    {
                        bot_id = botId,
                        updating_by = updatingBy,
                        update_avatar = updateAvatar,
                        update_description = updateDescription,
                        name = newName,
                        tag = newTag,
                        description = newDescription
                    };
                }
                else
                {
                    sql = """
                        SELECT sch_user.update_bot(
                            @bot_id,
                            @updating_by,
                            @update_avatar,
                            @update_description,
                            @name,
                            @tag,
                            ROW(@media_id, @file_name, @content_type)::media_file,
                            @description)
                        """;
                    param = new
                    {
                        bot_id = botId,
                        updating_by = updatingBy,
                        update_avatar = updateAvatar,
                        update_description = updateDescription,
                        name = newName,
                        tag = newTag,
                        description = newDescription,
                        media_id = newAvatar.MediaId,
                        file_name = newAvatar.FileName,
                        content_type = newAvatar.ContentType
                    };
                }

                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, param, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task UpdateCommandArgumentAsync(Guid botId, Guid updatingBy, uint commandId, uint argumentId, string? newName,
            string? newType, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = """
                    SELECT sch_user.update_command_argument(
                        @bot_id,
                        @updating_by,
                        @command_id,
                        @argument_id,
                        @new_name,
                        @new_type)
                    """;
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    bot_id = botId,
                    updating_by = updatingBy,
                    command_id = (int)commandId,
                    argument_id = (int)argumentId,
                    new_name = newName,
                    new_type = newType
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }

        public async Task UpdateCommandAsync(Guid botId, Guid updatingBy, uint commandId, char? newPrefix, string? newCommand,
            string? newCommandDescription, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = """
                    SELECT sch_user.update_command(
                        @bot_id,
                        @updating_by,
                        @command_id,
                        @new_prefix,
                        @new_command,
                        @new_description)
                    """;
                await conn.ExecuteScalarAsync<int>(RepositoryExecution.Cmd(sql, new
                {
                    bot_id = botId,
                    updating_by = updatingBy,
                    command_id = (int)commandId,
                    new_prefix = newPrefix,
                    new_command = newCommand,
                    new_description = newCommandDescription
                }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }
    }
}
