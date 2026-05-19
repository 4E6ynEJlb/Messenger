using System.Security.Claims;

namespace UserAPI.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid GetUserId(this HttpContext context)
        {
            var userIdClaim = context.User.FindFirst(ClaimsIdentity.DefaultNameClaimType)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new Exception("User ID claim is missing or invalid.");
            }
            return userId;
        }
    }
}
