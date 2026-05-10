using Application.Models.Internal.Constants;

namespace UserAPI.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static void AppendRefreshToken(this HttpResponse response, string token, int expirationHours)
        {
            response.Cookies.Append(CookiesKeys.REFRESH_TOKEN, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(expirationHours)
            });
        }

        public static void AppendDeviceId(this HttpResponse response, Guid deviceId, int expirationHours)
        {
            response.Cookies.Append(CookiesKeys.DEVICE_ID, deviceId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(expirationHours)
            });
        }

        public static void AppendUserId(this HttpResponse response, Guid userId, int expirationHours)
        {
            response.Cookies.Append(CookiesKeys.USER_ID, userId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(expirationHours)
            });
        }

        public static void AppendExpiration(this HttpResponse response, int expirationHours)
        {
            response.Cookies.Append(CookiesKeys.EXPIRATION, expirationHours.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(expirationHours)
            });
        }

        public static string? TryGetRefreshToken(this HttpRequest request)
        {
            request.Cookies.TryGetValue(CookiesKeys.REFRESH_TOKEN, out string? refreshToken);
            return refreshToken;
        }

        public static Guid? TryGetDeviceId(this HttpRequest request)
        {
            request.Cookies.TryGetValue(CookiesKeys.DEVICE_ID, out string? deviceIdString);
            if (Guid.TryParse(deviceIdString, out Guid deviceId))
            {
                return deviceId;
            }
            return null;
        }

        public static Guid? TryGetUserId(this HttpRequest request)
        {
            request.Cookies.TryGetValue(CookiesKeys.USER_ID, out string? userIdString);
            if (Guid.TryParse(userIdString, out Guid userId))
            {
                return userId;
            }
            return null;
        }

        public static int? TryGetExpiration(this HttpRequest request)
        {
            request.Cookies.TryGetValue(CookiesKeys.EXPIRATION, out string? expirationString);
            if (int.TryParse(expirationString, out int expiration))
            {
                return expiration;
            }
            return null;
        }

        public static void DeleteRefreshToken(this HttpResponse response)
        {
            response.Cookies.Delete(CookiesKeys.REFRESH_TOKEN);
        }

        public static void DeleteDeviceId(this HttpResponse response)
        {
            response.Cookies.Delete(CookiesKeys.DEVICE_ID);
        }

        public static void DeleteUserId(this HttpResponse response)
        {
            response.Cookies.Delete(CookiesKeys.USER_ID);
        }

            public static void DeleteExpiration(this HttpResponse response)
            {
                response.Cookies.Delete(CookiesKeys.EXPIRATION);
        }
    }
}
