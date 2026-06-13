using Domain.Models.Types;
using Npgsql;

namespace Infrastructure.Database
{
    public class NpgSqlConnectionFactory : IDbConnectionFactory
    {
        private readonly NpgsqlDataSource _npgsqlDataSource;

        public NpgSqlConnectionFactory(NpgsqlDataSource dataSource)
        {
            _npgsqlDataSource = dataSource;
        }

        public async Task<NpgsqlConnection> CreateConnectionAsync()
        {
            return await _npgsqlDataSource.OpenConnectionAsync();
        }
    }
}
