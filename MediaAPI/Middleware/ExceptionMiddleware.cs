using Domain;
using System.Net;
using System.Text.Json;

namespace MediaAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (HttpException httpException)
            {
                await HandleHttpException(context, httpException);
            }
            catch (Exception exception)
            {
                 await HandleUnknownException(context, exception);
            }
        }

        private static Task HandleHttpException(HttpContext context, HttpException exception)
        {
            int statusCode = (int)exception.StatusCode;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            string result = JsonSerializer.Serialize(new
            {
                StatusCode = statusCode,
                ErrorMessage = exception.Message
            });
            return context.Response.WriteAsync(result);
        }

        private static Task HandleUnknownException(HttpContext context, Exception exception)
        {
            int statusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            string result = JsonSerializer.Serialize(new
            {
                StatusCode = statusCode,
                ErrorMessage = exception.Message
            });
            return context.Response.WriteAsync(result);
        }
    }
}
