using Domain;
using System.Net;

namespace Persistence.Exceptions
{
    public sealed class UserBannedInChatException : HttpException
    {
        public UserBannedInChatException(Exception innerException)
            : base(innerException, ErrorMessages.USER_BANNED_IN_PUBLIC_CHAT, HttpStatusCode.Forbidden)
        {
        }
    }
}
