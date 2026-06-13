using Dapper;
using Npgsql;

namespace Persistence
{
    internal static class RepositoryExecution
    {
        public static CommandDefinition Cmd(string sql, object? param, CancellationToken cancellationToken,
            NpgsqlTransaction? transaction = null) =>
            new(sql, param, transaction: transaction, cancellationToken: cancellationToken);
    }
}
