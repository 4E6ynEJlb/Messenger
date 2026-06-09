using Npgsql;

namespace Infrastructure.Database
{
    public interface IDbConnectionFactory
    {
        public Task<NpgsqlConnection> CreateConnectionAsync();
    }
}
