CREATE TABLE IF NOT EXISTS private.media
(
    media_id uuid PRIMARY KEY,
    file_name varchar(260) NOT NULL,
    content_type varchar(24) NOT NULL,
    links_count int NOT NULL DEFAULT 1
);


CREATE TABLE IF NOT EXISTS private.deleted_media_list
(
    media_id uuid PRIMARY KEY
);


CREATE TABLE IF NOT EXISTS private.users
(
    user_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    login_hash bytea UNIQUE NOT NULL,
    password_hash bytea NOT NULL,
    first_name varchar(32) NOT NULL,
    last_name varchar(32) NOT NULL,
    tag varchar(16) UNIQUE NOT NULL,
    avatar uuid,
    birth_date date NOT NULL,
    was_online timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    bio varchar(512),
    FOREIGN KEY (avatar) REFERENCES private.media(media_id)
);


CREATE TABLE IF NOT EXISTS private.bots
(
    bot_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    token_hash bytea UNIQUE NOT NULL,
    token_version dom_positive_int NOT NULL DEFAULT 1,
    name varchar(32) NOT NULL,
    tag varchar(16) UNIQUE NOT NULL,
    avatar uuid,
    description varchar(512),
    owner uuid,
    is_enabled boolean NOT NULL DEFAULT true,
    FOREIGN KEY (avatar) REFERENCES private.media(media_id) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (owner) REFERENCES private.users(user_id) ON DELETE SET NULL ON UPDATE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_bots_name_trgm
ON private.bots
USING gin (name gin_trgm_ops);

CREATE INDEX IF NOT EXISTS bots_owner_hash_idx ON private.bots USING hash(owner);


CREATE TABLE IF NOT EXISTS private.avatars
(
    media_id uuid NOT NULL,
    user_id uuid NOT NULL,
    uploaded_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (media_id, user_id)
) PARTITION BY LIST (user_id);


CREATE TABLE IF NOT EXISTS private.administrators
(
    administrator_id serial PRIMARY KEY,
    assigned_at timestamp DEFAULT CURRENT_TIMESTAMP NOT NULL,
    login_hash bytea UNIQUE NOT NULL,
    password_hash bytea UNIQUE NOT NULL
);


CREATE TABLE IF NOT EXISTS private.banned_users
(
    user_id uuid PRIMARY KEY,
    banned_by int,
    banned_at timestamp DEFAULT CURRENT_TIMESTAMP NOT NULL,
    unbanned_at timestamp NOT NULL CHECK (unbanned_at > banned_at),
    reason varchar(512) NOT NULL,
    FOREIGN KEY (user_id) REFERENCES private.users(user_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (banned_by) REFERENCES private.administrators(administrator_id) ON DELETE SET NULL ON UPDATE CASCADE
);

CREATE INDEX IF NOT EXISTS banned_users_unbanned_at_btree_idx ON private.banned_users(unbanned_at);


CREATE TABLE IF NOT EXISTS private.personal_chats
(
    chat_id uuid PRIMARY KEY
);


CREATE TABLE IF NOT EXISTS private.personal_chats_members
(
    chat_id uuid NOT NULL,
    user_id uuid,
    was_in_chat timestamp NOT NULL default CURRENT_TIMESTAMP,
    FOREIGN KEY (chat_id) REFERENCES private.personal_chats(chat_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (user_id) REFERENCES private.users(user_id) ON DELETE SET NULL ON UPDATE CASCADE,
    PRIMARY KEY (chat_id, user_id)
);


CREATE TABLE IF NOT EXISTS private.personal_messages
(
    message_id uuid NOT NULL,
    chat_id uuid NOT NULL,
    author uuid,
    message_text text NOT NULL,
    sent_at timestamp DEFAULT CURRENT_TIMESTAMP,
    is_updated boolean DEFAULT false,
    updated_at timestamp DEFAULT NULL,
    reply_to uuid DEFAULT NULL CHECK (reply_to IS NULL OR reply_to != message_id),
    resent_from uuid DEFAULT NULL,
    is_bot_resend bool DEFAULT NULL,
    PRIMARY KEY (chat_id, message_id)
) PARTITION BY LIST (chat_id);

CREATE INDEX IF NOT EXISTS personal_messages_sent_at_btree_idx ON private.personal_messages(sent_at);


CREATE TABLE IF NOT EXISTS private.personal_messages_attachments
(
    attachment_id uuid NOT NULL,
    message_id uuid NOT NULL,
    chat_id uuid NOT NULL,
    PRIMARY KEY (chat_id, attachment_id)
) PARTITION BY LIST (chat_id);

CREATE INDEX IF NOT EXISTS personal_messages_attachments_message_id_hash_idx ON private.personal_messages_attachments USING HASH(message_id);


CREATE TABLE IF NOT EXISTS private.public_chats
(
    chat_id uuid PRIMARY KEY,
    chat_name dom_public_chat_name,
    default_member_role en_public_chat_member_role NOT NULL DEFAULT 'Member',
    is_searchable bool NOT NULL DEFAULT false,
    avatar uuid,
    FOREIGN KEY (avatar) REFERENCES private.media(media_id) ON UPDATE CASCADE ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS idx_public_chats_name_trgm
ON private.public_chats
USING gin (chat_name gin_trgm_ops);


CREATE TABLE IF NOT EXISTS private.public_chats_members
(
    chat_id uuid NOT NULL,
    user_id uuid,
    role en_public_chat_member_role DEFAULT 'Member',
    was_in_chat timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (chat_id, user_id)
) PARTITION BY LIST(chat_id);

CREATE INDEX IF NOT EXISTS public_chats_members_chat_id_hash_idx ON private.public_chats_members USING hash(chat_id);


CREATE TABLE IF NOT EXISTS private.public_chats_banned_users
(
    chat_id uuid NOT NULL,
    user_id uuid,
    banned_by uuid,
    banned_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (chat_id, user_id)
) PARTITION BY LIST(chat_id);

CREATE TABLE IF NOT EXISTS private.public_messages
(
    message_id uuid,
    chat_id uuid,
    author uuid,
    message_text text NOT NULL,
    sent_at timestamp DEFAULT CURRENT_TIMESTAMP,
    is_updated boolean DEFAULT false,
    updated_at timestamp DEFAULT NULL,
    reply_to uuid DEFAULT NULL CHECK (reply_to IS NULL OR reply_to != message_id),
    resent_from uuid DEFAULT NULL,
    is_bot_resend bool DEFAULT NULL,
    PRIMARY KEY (chat_id, message_id)
) PARTITION BY LIST (chat_id);

CREATE INDEX IF NOT EXISTS public_messages_sent_at_btree_idx ON private.public_messages(sent_at);


CREATE TABLE IF NOT EXISTS private.public_messages_attachments
(
    attachment_id uuid,
    message_id uuid,
    chat_id uuid,
    PRIMARY KEY (chat_id, attachment_id)
) PARTITION BY LIST (chat_id);

CREATE INDEX IF NOT EXISTS public_messages_attachments_message_id_hash_idx ON private.public_messages_attachments USING hash(message_id);


CREATE TABLE IF NOT EXISTS private.public_chats_audit_logs
(
    chat_id uuid,
    action_datetime timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    source_user_id uuid,
    destination_user_id uuid,
    action varchar(128),
    PRIMARY KEY (chat_id, action_datetime)
) PARTITION BY LIST (chat_id);

CREATE INDEX IF NOT EXISTS public_chats_audit_logs_action_datetime_btree_idx ON private.public_chats_audit_logs(action_datetime);

CREATE TABLE IF NOT EXISTS private.administrators_actions_log
(
    action_datetime timestamp PRIMARY KEY DEFAULT CURRENT_TIMESTAMP,
    administrator_id int,
    action varchar(128)
) PARTITION BY RANGE(action_datetime);


CREATE TABLE IF NOT EXISTS private.user_reports
(
    report_id uuid DEFAULT gen_random_uuid(),
    report_datetime timestamp DEFAULT CURRENT_TIMESTAMP,
    reported_by uuid,
    comment varchar(2048)
);


CREATE TABLE IF NOT EXISTS private.users_message_reports--trigger-check chat_type+chat_id---------------------------------------------------
(
    chat_type en_chat_type,
    chat_id uuid,
    message_id uuid,
    PRIMARY KEY (report_id),
    CHECK (report_datetime IS NOT NULL),
    FOREIGN KEY (reported_by) REFERENCES private.users(user_id) ON DELETE SET NULL ON UPDATE CASCADE
) INHERITS (private.user_reports);

CREATE INDEX IF NOT EXISTS users_message_reports_report_datetime_btree_idx ON private.users_message_reports(report_datetime);


CREATE TABLE IF NOT EXISTS private.users_user_reports
(
    reported_user_id uuid,
    PRIMARY KEY (report_id),
    CHECK (report_datetime IS NOT NULL),
    FOREIGN KEY (reported_by) REFERENCES private.users(user_id) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (reported_user_id) REFERENCES private.users(user_id) ON DELETE SET NULL ON UPDATE CASCADE
) INHERITS (private.user_reports);

CREATE INDEX IF NOT EXISTS users_user_reports_report_datetime_btree_idx ON private.users_user_reports(report_datetime);


CREATE TABLE IF NOT EXISTS private.users_public_chat_reports
(
    chat_id uuid,
    PRIMARY KEY (report_id),
    CHECK (report_datetime IS NOT NULL),
    FOREIGN KEY (reported_by) REFERENCES private.users(user_id) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (chat_id) REFERENCES private.public_chats(chat_id) ON DELETE SET NULL ON UPDATE CASCADE
) INHERITS (private.user_reports);

CREATE INDEX IF NOT EXISTS users_public_chat_reports_report_datetime_btree_idx ON private.users_public_chat_reports(report_datetime);


CREATE TABLE IF NOT EXISTS private.users_administration_reports
(
    administrator_id int,
    PRIMARY KEY (report_id),
    CHECK (report_datetime IS NOT NULL),
    FOREIGN KEY (reported_by) REFERENCES private.users(user_id) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (administrator_id) REFERENCES private.administrators ON DELETE SET NULL ON UPDATE CASCADE
) INHERITS (private.user_reports);

CREATE INDEX IF NOT EXISTS users_administration_reports_report_datetime_btree_idx ON private.users_administration_reports(report_datetime);


CREATE TABLE IF NOT EXISTS private.users_bot_reports
(
    bot_id uuid,
    PRIMARY KEY (report_id),
    CHECK (report_datetime IS NOT NULL),
    FOREIGN KEY (reported_by) REFERENCES private.users(user_id) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (bot_id) REFERENCES private.bots(bot_id) ON DELETE SET NULL ON UPDATE CASCADE
) INHERITS (private.user_reports);

CREATE INDEX IF NOT EXISTS users_bot_reports_report_datetime_btree_idx ON private.users_bot_reports(report_datetime);


CREATE TABLE IF NOT EXISTS private.users_blocks
(
    user_id uuid NOT NULL,
    block_by uuid NOT NULL,
    CHECK (user_id != block_by),
    PRIMARY KEY (user_id, block_by),
    FOREIGN KEY (user_id) REFERENCES private.users(user_id) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (block_by) REFERENCES private.users(user_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS private.bots_connections_logs
(
    bot_id uuid NOT NULL,
    ip_address inet NOT NULL,
    connected_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    token_version dom_positive_int NOT NULL,
    FOREIGN KEY (bot_id) REFERENCES private.bots(bot_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE INDEX IF NOT EXISTS bots_connections_logs_bot_id_hash_idx ON private.bots_connections_logs USING hash(bot_id);
CREATE INDEX IF NOT EXISTS bots_connections_logs_connected_at_btree_idx ON private.bots_connections_logs(connected_at);


CREATE TABLE IF NOT EXISTS private.bots_commands
(
    bot_id uuid NOT NULL,
    command_id dom_positive_int NOT NULL,
    prefix char NOT NULL CHECK (prefix !~ '[A-Za-zА-Яа-яЁё0-9]'),
    command varchar(8) NOT NULL CHECK (command ~ '^[A-Za-z][A-Za-z0-9]{0,7}$'),
    description varchar(32),
    UNIQUE (bot_id, prefix, command),
    PRIMARY KEY (bot_id, command_id),
    FOREIGN KEY (bot_id) REFERENCES private.bots(bot_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE INDEX IF NOT EXISTS bots_commands_bot_id_hash_idx ON private.bots_commands USING hash(bot_id);


CREATE TABLE IF NOT EXISTS private.bots_commands_arguments
(
    bot_id uuid NOT NULL,
    command_id dom_positive_int NOT NULL,
    argument_id dom_positive_int NOT NULL,
    name varchar(32) NOT NULL,
    type varchar(32) NOT NULL,
    PRIMARY KEY (bot_id, command_id, argument_id),
    FOREIGN KEY (bot_id, command_id) REFERENCES private.bots_commands(bot_id, command_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS private.bot_chats
(
    chat_id uuid PRIMARY KEY,
    bot_id uuid,
    user_id uuid,
    was_in_chat timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_enabled bool NOT NULL DEFAULT false,
    UNIQUE (bot_id, user_id),
    FOREIGN KEY (bot_id) REFERENCES private.bots(bot_id) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (user_id) REFERENCES private.users(user_id) ON DELETE SET NULL ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS private.bot_messages
(
    message_id uuid,
    chat_id uuid,
    is_bot bool NOT NULL,
    message_text text NOT NULL,
    sent_at timestamp DEFAULT CURRENT_TIMESTAMP,
    is_updated boolean DEFAULT false,
    updated_at timestamp DEFAULT NULL,
    reply_to uuid DEFAULT NULL CHECK (reply_to IS NULL OR reply_to != message_id),
    resent_from uuid DEFAULT NULL,
    is_bot_resend bool DEFAULT NULL,
    is_handled bool NOT NULL DEFAULT false,
    PRIMARY KEY (chat_id, message_id)
) PARTITION BY LIST (chat_id);

CREATE INDEX IF NOT EXISTS bot_messages_sent_at_btree_idx ON private.bot_messages(sent_at);
CREATE INDEX bot_messages_unhandled_sorted_partial_idx ON private.bot_messages (chat_id, sent_at) WHERE is_handled = false;

CREATE TABLE IF NOT EXISTS private.bot_messages_attachments
(
    attachment_id uuid,
    message_id uuid,
    chat_id uuid,
    PRIMARY KEY (chat_id, attachment_id)
) PARTITION BY LIST (chat_id);

CREATE TABLE IF NOT EXISTS private.bot_chats_active_buttons
(
    chat_id uuid NOT NULL,
    button_id dom_positive_int NOT NULL,
    button_text varchar(16) NOT NULL,
    inner_command varchar(16) NOT NULL,
    background_color bytea CHECK (background_color IS NULL OR length(background_color) = 3),
    PRIMARY KEY (chat_id, button_id)
) PARTITION BY LIST (chat_id);

CREATE TABLE IF NOT EXISTS private.users_refresh_tokens
(
    refresh_token varchar(44) NOT NULL PRIMARY KEY CHECK (length(refresh_token) = 44),
    user_id uuid NOT NULL,
    device_id uuid NOT NULL UNIQUE,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    lifetime interval NOT NULL,
    FOREIGN KEY (user_id) REFERENCES private.users(user_id)
);

CREATE INDEX IF NOT EXISTS users_refresh_tokens_expiration_btree_idx ON private.users_refresh_tokens((created_at + lifetime));
CREATE INDEX IF NOT EXISTS users_refresh_tokens_user_id_hash_idx ON private.users_refresh_tokens USING hash(user_id);