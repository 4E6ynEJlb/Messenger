using Domain;
using System.Net;

namespace Persistence.Exceptions
{
    public sealed class ResourceNotFoundException : HttpException
    {
        public ResourceNotFoundException(Exception innerException)
            : base(innerException, ErrorMessages.RESOURCE_NOT_FOUND, HttpStatusCode.NotFound)
        {
        }
    }
}
