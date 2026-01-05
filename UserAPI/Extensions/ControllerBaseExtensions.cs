using Microsoft.AspNetCore.Mvc;

namespace UserAPI.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static OkResult NewJwtToken(this ControllerBase controller)
        {
            return controller.Ok();
        }

        public static void AppendRefreshToken(this HttpResponse response)
        {
            response.Cookies.Append("example_cookie", "example_value", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
        }

        public static string? TryGetRefreshToken(this HttpRequest request)
        {
            request.Cookies.TryGetValue("example_cookie", out string? refreshToken);
            return refreshToken;
        }
    }
}
