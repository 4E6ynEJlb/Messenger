using Application.Models.Internal.Constants;

namespace UserAPI.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static void AppendRefreshToken(this HttpResponse response, string token, int expirationDays)
        {
            response.Cookies.Append(CookiesKeys.REFRESH_TOKEN, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(expirationDays)
            });
        }

        public static void AppendCredentials(this HttpResponse response, string encryptedCredentials)
        {
            response.Cookies.Append(CookiesKeys.CREDENTIALS, encryptedCredentials, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        }

        public static string? TryGetRefreshToken(this HttpRequest request)
        {
            request.Cookies.TryGetValue(CookiesKeys.REFRESH_TOKEN, out string? refreshToken);
            return refreshToken;
        }

        public static string? TryGetCredentials(this HttpRequest request)
        {
            request.Cookies.TryGetValue(CookiesKeys.CREDENTIALS, out string? encryptedCredentials);
            return encryptedCredentials;
        }

        public static void DeleteRefreshToken(this HttpResponse response)
        {
            response.Cookies.Delete(CookiesKeys.REFRESH_TOKEN);
        }

        public static void DeleteCredentials(this HttpResponse response)
        {
            response.Cookies.Delete(CookiesKeys.CREDENTIALS);
        }
    }
}
