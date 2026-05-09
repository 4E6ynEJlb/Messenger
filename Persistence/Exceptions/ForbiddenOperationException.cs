using Domain;
using System.Net;

namespace Persistence.Exceptions
{
    public sealed class ForbiddenOperationException : HttpException
    {
        public ForbiddenOperationException(Exception innerException, string message)
            : base(innerException, message, HttpStatusCode.Forbidden)
        {
        }
    }
}
