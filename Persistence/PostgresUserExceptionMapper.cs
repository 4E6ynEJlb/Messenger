using Domain;
using Npgsql;
using Persistence.Exceptions;

namespace Persistence
{
    internal static class PostgresUserExceptionMapper
    {
        internal static Exception For(PostgresException ex)
        {
            if (ex.SqlState == PostgresErrorCodes.NO_DATA_FOUND)
                return new ResourceNotFoundException(ex);

            if (IsInvalidUserInputSqlState(ex.SqlState))
                return new InvalidUserInputException(ex);

            if (ex.Message == ErrorMessages.REFRESH_TOKEN_INVALID)
                return new InvalidRefreshTokenException(ex);

            if (ex.Message == ErrorMessages.USER_BANNED_IN_PUBLIC_CHAT)
                return new UserBannedInChatException(ex);

            if (ex.SqlState == PostgresErrorCodes.INSUFFICIENT_PRIVILEGE
                || ex.SqlState == PostgresErrorCodes.INVALID_AUTHORIZATION_SPECIFICATION)
            {
                var msg = string.IsNullOrEmpty(ex.Message)
                    ? ErrorMessages.FORBIDDEN_OPERATION
                    : ex.Message;
                return new ForbiddenOperationException(ex, msg);
            }

            return new InvalidOperationException(ex.Message, ex);
        }

        private static bool IsInvalidUserInputSqlState(string? sqlState)
        {
            return sqlState is PostgresErrorCodes.DATA_EXCEPTION
                or PostgresErrorCodes.CHECK_VIOLATION
                or PostgresErrorCodes.NOT_NULL_VIOLATION
                or PostgresErrorCodes.FOREIGN_KEY_VIOLATION
                or PostgresErrorCodes.UNIQUE_VIOLATION
                or PostgresErrorCodes.STRING_DATA_RIGHT_TRUNCATION;
        }

        private static class PostgresErrorCodes
        {
            internal const string NO_DATA_FOUND = "P0002";
            internal const string DATA_EXCEPTION = "22000";
            internal const string CHECK_VIOLATION = "23514";
            internal const string NOT_NULL_VIOLATION = "23502";
            internal const string FOREIGN_KEY_VIOLATION = "23503";
            internal const string UNIQUE_VIOLATION = "23505";
            internal const string STRING_DATA_RIGHT_TRUNCATION = "22001";
            internal const string INSUFFICIENT_PRIVILEGE = "42501";
            internal const string INVALID_AUTHORIZATION_SPECIFICATION = "28000";
        }
    }
}
