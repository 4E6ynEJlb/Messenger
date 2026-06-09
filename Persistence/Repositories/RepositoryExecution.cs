using Dapper;
using Npgsql;

namespace Persistence.Repositories
{
    internal static class RepositoryExecution
    {
        public static CommandDefinition Cmd(string sql, object? param, CancellationToken cancellationToken) =>
            new(sql, param, cancellationToken: cancellationToken);
    }
}
