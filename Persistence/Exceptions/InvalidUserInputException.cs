using Domain;
using System.Net;

namespace Persistence.Exceptions
{
    public sealed class InvalidUserInputException : HttpException
    {
        public InvalidUserInputException(Exception innerException)
            : base(innerException, ErrorMessages.INVALID_REQUEST_DATA, HttpStatusCode.BadRequest)
        {
        }
    }
}
