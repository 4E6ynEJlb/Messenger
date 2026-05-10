using Domain;
using System.Net;

namespace Persistence.Exceptions
{
    public sealed class InvalidRefreshTokenException : HttpException
    {
        public InvalidRefreshTokenException(Exception innerException)
            : base(innerException, ErrorMessages.REFRESH_TOKEN_INVALID, HttpStatusCode.Unauthorized)
        {
        }
    }
}
