namespace Domain
{
    public static class ErrorMessages
    {
        public const string FILE_DOES_NOT_EXIST = "File does not exist.";

        public const string REFRESH_TOKEN_INVALID = "Invalid or revoked refresh token.";

        public const string PERSONAL_CHAT_BLOCKED = "This action is not allowed while users are blocked in this personal chat.";

        public const string PERSONAL_MESSAGE_FORWARDED_EDIT_DENIED = "Forwarded personal messages cannot be edited.";

        public const string PERSONAL_FORWARDED_ATTACHMENT_DELETE_DENIED =
            "Attachments cannot be removed from forwarded personal messages.";

        public const string PUBLIC_CHAT_MEMBERSHIP_REQUIRED = "You must be a member of this public chat.";

        public const string PUBLIC_CHAT_READER_CANNOT_POST = "Readers cannot send messages in this public chat.";

        public const string PUBLIC_CHAT_READER_ACTION_FORBIDDEN = "Readers cannot perform this action in the public chat.";

        public const string PUBLIC_FORWARDED_MESSAGE_EDIT_DENIED = "Forwarded public messages cannot be edited.";

        public const string PUBLIC_MESSAGE_DELETE_FORBIDDEN = "You cannot delete this public message.";

        public const string PUBLIC_FORWARDED_ATTACHMENT_DELETE_DENIED =
            "Attachments cannot be removed from forwarded public messages.";

        public const string PUBLIC_CHAT_DELETE_CREATOR_ONLY = "Only the chat creator can delete this public chat.";

        public const string PUBLIC_ROLE_CHANGE_FORBIDDEN = "You cannot change member roles in this public chat.";

        public const string PUBLIC_CANNOT_ASSIGN_CREATOR_ROLE = "Only the creator can assign the creator role.";

        public const string PUBLIC_KICK_FORBIDDEN = "You cannot remove this member from the public chat.";

        public const string PUBLIC_CANNOT_KICK_SELF = "You cannot remove yourself from the public chat this way.";

        public const string PUBLIC_KICK_PEER_ADMINS_FORBIDDEN =
            "Administrators cannot kick other administrators or the creator.";

        public const string PUBLIC_SETTINGS_UPDATE_FORBIDDEN =
            "Only administrators or the creator can update public chat settings.";

        public const string PUBLIC_BAN_FORBIDDEN = "You cannot ban this user in the public chat.";

        public const string PUBLIC_CANNOT_BAN_SELF = "You cannot ban yourself in the public chat.";

        public const string PUBLIC_BAN_PEER_ADMINS_FORBIDDEN =
            "Administrators cannot ban other administrators or the creator.";

        public const string PUBLIC_UNBAN_FORBIDDEN = "You cannot unban this user in the public chat.";

        public const string PUBLIC_UNBAN_PEER_ADMINS_FORBIDDEN =
            "Administrators cannot unban administrators or the creator.";

        public const string PUBLIC_CREATOR_CANNOT_LEAVE = "The chat creator cannot leave the public chat.";

        public const string USER_BANNED_IN_PUBLIC_CHAT = "User is banned in chat";

        public const string BOT_CHAT_BOT_DISABLED = "The bot is disabled in this chat.";

        public const string RESOURCE_NOT_FOUND = "The requested resource was not found.";

        public const string INVALID_REQUEST_DATA = "The request data is invalid.";

        public const string FORBIDDEN_OPERATION = "This operation is not allowed.";
    }
}
