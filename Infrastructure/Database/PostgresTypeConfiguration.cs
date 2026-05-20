using Domain.Models.Types;
using Npgsql;

namespace Infrastructure.Database
{
    public static class PostgresTypeConfiguration
    {
        public static NpgsqlDataSourceBuilder ConfigureMessengerPostgresTypes(this NpgsqlDataSourceBuilder builder)
        {
            builder.MapEnum<EnChatType>("en_chat_type", new EnChatTypePgTranslator());
            builder.MapEnum<EnPublicChatMemberRole>("en_public_chat_member_role", new EnPublicChatMemberRolePgTranslator());
            builder.MapEnum<EnPublicChatAuditRecordAction>("en_public_chat_audit_record_action",
                new EnPublicChatAuditRecordActionPgTranslator());

            builder.MapComposite<UserData>("user_data");
            builder.MapComposite<MediaFile>("media_file");
            builder.MapComposite<ChatInformation>("chat_information");
            builder.MapComposite<ChatMemberInfo>("chat_member_info");
            builder.MapComposite<PublicChatFullInformation>("public_chat_full_information");
            builder.MapComposite<PublicChatOptions>("public_chat_options");
            builder.MapComposite<PublicChatBannedUser>("public_chat_banned_user");
            builder.MapComposite<AuditLogRecord>("audit_log_record");
            builder.MapComposite<BotInfo>("bot_info");
            builder.MapComposite<BotTokenInfo>("bot_token_info");
            builder.MapComposite<BotCommandArgument>("bot_command_argument");
            builder.MapComposite<BotCommandInfo>("bot_command_info");
            builder.MapComposite<BotConnectionLogRecord>("bot_connection_log_record");
            builder.MapComposite<BotButtonInfo>("bot_button_info");
            return builder;
        }

        private sealed class EnChatTypePgTranslator : INpgsqlNameTranslator
        {
            public string TranslateTypeName(string clrName) => "en_chat_type";

            public string TranslateMemberName(string clrName) => clrName switch
            {
                nameof(EnChatType.Personal) => "Personal",
                nameof(EnChatType.Public) => "Public",
                nameof(EnChatType.Bot) => "Bot",
                _ => clrName
            };
        }

        private sealed class EnPublicChatMemberRolePgTranslator : INpgsqlNameTranslator
        {
            public string TranslateTypeName(string clrName) => "en_public_chat_member_role";

            public string TranslateMemberName(string clrName) => clrName switch
            {
                nameof(EnPublicChatMemberRole.Creator) => "Creator",
                nameof(EnPublicChatMemberRole.Administrator) => "Administrator",
                nameof(EnPublicChatMemberRole.Member) => "Member",
                nameof(EnPublicChatMemberRole.Reader) => "Reader",
                _ => clrName
            };
        }

        private sealed class EnPublicChatAuditRecordActionPgTranslator : INpgsqlNameTranslator
        {
            public string TranslateTypeName(string clrName) => "en_public_chat_audit_record_action";

            public string TranslateMemberName(string clrName) => clrName;
        }
    }
}
