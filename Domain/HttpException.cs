using System.Net;

namespace Domain
{
    public abstract class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public HttpException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpException(Exception innerException, string message, HttpStatusCode statusCode) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
