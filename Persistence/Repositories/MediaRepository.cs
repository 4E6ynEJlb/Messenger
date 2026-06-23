using Dapper;
using Domain.Models.Types;
using Domain.Stores;
using Infrastructure.Database;
using Npgsql;

namespace Persistence.Repositories
{
    public class MediaRepository : IMediaStore
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MediaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<MediaFile?> GetOneByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await using var conn = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                const string sql = "SELECT sch_media.get_media_by_id(@media_id)";
                return await conn.ExecuteScalarAsync<MediaFile?>(RepositoryExecution.Cmd(sql,
                    new { media_id = id }, cancellationToken)).ConfigureAwait(false);
            }
            catch (PostgresException ex)
            {
                throw PostgresUserExceptionMapper.For(ex);
            }
        }
    }
}
