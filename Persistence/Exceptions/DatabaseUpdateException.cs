using Domain;
using System.Net;

namespace Persistence.Exceptions
{
    public sealed class DatabaseUpdateException : HttpException
    {
        public DatabaseUpdateException(Exception innerException)
            : base(innerException, ErrorMessages.NO_CHANGES_APPLIED, HttpStatusCode.NotFound)
        {
        }
    }
}
