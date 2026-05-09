using Domain;
using System.Net;

namespace Infrastructure.Exceptions
{
    public class FileDoesNotExistException : HttpException
    {
        public FileDoesNotExistException(Exception innerException) : 
            base(innerException, ErrorMessages.FILE_DOES_NOT_EXIST, HttpStatusCode.NotFound)
        { }
    }
}
