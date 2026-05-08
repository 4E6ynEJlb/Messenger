CREATE SCHEMA IF NOT EXISTS private;
CREATE SCHEMA IF NOT EXISTS sch_user;
CREATE SCHEMA IF NOT EXISTS sch_bot;
CREATE SCHEMA IF NOT EXISTS sch_administrator;

CREATE EXTENSION IF NOT EXISTS pg_trgm;

DO
$$
BEGIN
    IF NOT exists(SELECT 1 FROM pg_catalog.pg_roles WHERE rolname = 'user_web_api') THEN
        CREATE ROLE user_web_api WITH
        LOGIN
        PASSWORD 'user_web_api_password';
    END IF;
END
$$;

DO
$$
BEGIN
    IF NOT exists(SELECT 1 FROM pg_catalog.pg_roles WHERE rolname = 'bot_web_api') THEN
        CREATE ROLE bot_web_api WITH
        LOGIN
        PASSWORD 'bot_web_api_password';
    END IF;
END
$$;

DO
$$
BEGIN
    IF NOT exists(SELECT 1 FROM pg_catalog.pg_roles WHERE rolname = 'administrator_web_api') THEN
        CREATE ROLE administrator_web_api WITH
        LOGIN
        PASSWORD 'administrator_web_api_password';
    END IF;
END
$$;

--запрет использования всего из public, кроме типов
REVOKE ALL ON SCHEMA public FROM user_web_api;
REVOKE ALL ON SCHEMA public FROM bot_web_api;
REVOKE ALL ON SCHEMA public FROM administrator_web_api;

GRANT USAGE ON SCHEMA public TO user_web_api;
GRANT USAGE ON SCHEMA public TO bot_web_api;
GRANT USAGE ON SCHEMA public TO administrator_web_api;

REVOKE ALL ON ALL TABLES IN SCHEMA public FROM user_web_api;
REVOKE ALL ON ALL TABLES IN SCHEMA public FROM bot_web_api;
REVOKE ALL ON ALL TABLES IN SCHEMA public FROM administrator_web_api;

REVOKE ALL ON ALL SEQUENCES IN SCHEMA public FROM user_web_api;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA public FROM bot_web_api;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA public FROM administrator_web_api;

REVOKE ALL ON ALL FUNCTIONS IN SCHEMA public FROM user_web_api;
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA public FROM bot_web_api;
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA public FROM administrator_web_api;


--запрет использования всего из private
REVOKE ALL ON SCHEMA private FROM user_web_api;
REVOKE ALL ON SCHEMA private FROM bot_web_api;
REVOKE ALL ON SCHEMA private FROM administrator_web_api;

REVOKE ALL ON ALL TABLES IN SCHEMA private FROM user_web_api;
REVOKE ALL ON ALL TABLES IN SCHEMA private FROM bot_web_api;
REVOKE ALL ON ALL TABLES IN SCHEMA private FROM administrator_web_api;

REVOKE ALL ON ALL SEQUENCES IN SCHEMA private FROM user_web_api;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA private FROM bot_web_api;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA private FROM administrator_web_api;

REVOKE ALL ON ALL FUNCTIONS IN SCHEMA private FROM user_web_api;
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA private FROM bot_web_api;
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA private FROM administrator_web_api;


--запрет пользовательскому api на использование всего, кроме функций и процедур своей схемы
REVOKE ALL ON SCHEMA sch_user FROM user_web_api;
GRANT USAGE ON SCHEMA sch_user TO user_web_api;

REVOKE ALL ON ALL TABLES IN SCHEMA sch_user FROM user_web_api;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA sch_user FROM user_web_api;

REVOKE ALL ON ALL FUNCTIONS IN SCHEMA sch_user FROM user_web_api;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA sch_user TO user_web_api;

REVOKE ALL ON SCHEMA sch_bot FROM user_web_api;
REVOKE ALL ON SCHEMA sch_administrator FROM user_web_api;

REVOKE ALL ON ALL TABLES IN SCHEMA sch_bot FROM user_web_api;
REVOKE ALL ON ALL TABLES IN SCHEMA sch_administrator FROM user_web_api;

REVOKE ALL ON ALL SEQUENCES IN SCHEMA sch_bot FROM user_web_api;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA sch_administrator FROM user_web_api;

REVOKE ALL ON ALL FUNCTIONS IN SCHEMA sch_bot FROM user_web_api;
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA sch_administrator FROM user_web_api;


--запрет bot api на использование всего, кроме функций и процедур своей схемы
REVOKE ALL ON SCHEMA sch_bot FROM bot_web_api;
GRANT USAGE ON SCHEMA sch_bot TO bot_web_api;

REVOKE ALL ON ALL TABLES IN SCHEMA sch_bot FROM bot_web_api;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA sch_bot FROM bot_web_api;

REVOKE ALL ON ALL FUNCTIONS IN SCHEMA sch_bot FROM bot_web_api;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA sch_bot TO bot_web_api;

REVOKE ALL ON SCHEMA sch_administrator FROM bot_web_api;
REVOKE ALL ON ALL TABLES IN SCHEMA sch_administrator FROM bot_web_api;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA sch_administrator FROM bot_web_api;
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA sch_administrator FROM bot_web_api;


--запрет администраторскому api на использование всего, кроме функций и процедур своей схемы
REVOKE ALL ON SCHEMA sch_administrator FROM administrator_web_api;
GRANT USAGE ON SCHEMA sch_administrator TO administrator_web_api;

REVOKE ALL ON ALL TABLES IN SCHEMA sch_administrator FROM administrator_web_api;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA sch_administrator FROM administrator_web_api;

REVOKE ALL ON ALL FUNCTIONS IN SCHEMA sch_administrator FROM administrator_web_api;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA sch_administrator TO administrator_web_api;



DO
$$
BEGIN
    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'dom_auth_string') THEN
        CREATE DOMAIN dom_auth_string AS text
        CHECK (length(VALUE) BETWEEN 8 AND 16);
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'dom_positive_int') THEN
        CREATE DOMAIN dom_positive_int AS int
        CHECK (VALUE > 0);
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'dom_message_text') THEN
        CREATE DOMAIN dom_message_text AS text
        NOT NULL
        CHECK (length(trim(VALUE)) > 0);
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'dom_public_chat_name') THEN
        CREATE DOMAIN dom_public_chat_name AS varchar(64)
        CHECK (length(trim(VALUE)) > 0)
        NOT NULL;
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'en_chat_type') THEN
        CREATE TYPE en_chat_type AS ENUM
        (
            'Personal',
            'Public',
            'Bot'
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'en_public_chat_member_role') THEN
        CREATE TYPE en_public_chat_member_role AS ENUM
        (
            'Reader',
            'Member',
            'Administrator',
            'Creator'
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'en_public_chat_audit_record_action') THEN
        CREATE TYPE en_public_chat_audit_record_action AS ENUM
        (
            'Join',
            'UpdateMessage',
            'ChangeRole',
            'UpdateSettings',
            'Ban',
            'Unban',
            'DeleteMessage',
            'DeleteAttachment',
            'Leave',
            'Kick'
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'media_file') THEN
        CREATE TYPE media_file AS
        (
            media_id uuid,
            file_name varchar(260),
            content_type varchar(24)
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'user_data') THEN
        CREATE TYPE user_data AS
        (
            user_id uuid,
            first_name varchar(32),
            last_name varchar(32),
            tag varchar(16),
            avatar uuid,
            birth_date date,
            bio varchar(512),
            was_online timestamp
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'chat_information') THEN
        CREATE TYPE chat_information AS
        (
            chat_id uuid,
            chat_name varchar(65),
            new_messages_count integer,
            chat_image uuid,
            chat_type en_chat_type
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'message') THEN
        CREATE TYPE message AS
        (
            message_id uuid,
            author uuid,
            message_text text,
            sent_at timestamp,
            is_updated boolean,
            updated_at timestamp,
            reply_to uuid,
            resent_from uuid,
            is_bot_resend bool,
            attached_media uuid[]
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'chat_member_info') THEN
        CREATE TYPE chat_member_info AS
        (
            user_id uuid,
            full_name varchar(65),
            avatar uuid,
            role en_public_chat_member_role
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'public_chat_full_information') THEN
        CREATE TYPE public_chat_full_information AS
        (
            chat_name varchar(64),
            avatar uuid,
            members chat_member_info[]
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'public_chat_options') THEN
        CREATE TYPE public_chat_options AS
        (
            chat_name varchar(64),
            is_searchable bool,
            avatar uuid,
            default_member_role en_public_chat_member_role
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'public_chat_banned_user') THEN
        CREATE TYPE public_chat_banned_user AS
        (
            user_id uuid,
            banned_by uuid,
            banned_at timestamp
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'audit_log_record') THEN
        CREATE TYPE audit_log_record AS
        (
            action_datetime timestamp,
            source_user_id uuid,
            destination_user_id uuid,
            action en_public_chat_audit_record_action
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'bot_info') THEN
        CREATE TYPE bot_info AS
        (
            bot_id uuid,
            name varchar(32),
            tag varchar(16),
            avatar uuid,
            description varchar(512),
            is_enabled boolean
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'bot_token_info') THEN
        CREATE TYPE bot_token_info AS
        (
            token_hash bytea,
            token_version dom_positive_int
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'bot_command_argument') THEN
        CREATE TYPE bot_command_argument AS
        (
            argument_id dom_positive_int,
            name text,
            type text
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'bot_command_info') THEN
        CREATE TYPE bot_command_info AS
        (
            command_id dom_positive_int,
            prefix char,
            command varchar(8),
            description varchar(32),
            arguments bot_command_argument[]
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'bot_connection_log_record') THEN
        CREATE TYPE bot_connection_log_record AS
        (
            ip_address inet,
            connected_at timestamp,
            token_version dom_positive_int
        );
    END IF;

    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'bot_button_info') THEN
        CREATE TYPE bot_button_info AS
        (
            button_text varchar(16),
            inner_command varchar(16),
            background_color bytea
        );
    END IF;
END
$$;



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



CREATE OR REPLACE FUNCTION private.clear_deleted_media()
RETURNS SETOF uuid
AS
$$
    DELETE FROM private.deleted_media_list
    RETURNING media_id;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION private.delete_desolate_chats()
RETURNS int
AS
$$
DECLARE
    chats_count int;
    chats_id uuid[];
    deleting_chat_id uuid;
    chat_id_converted_to_text text;
BEGIN
    SELECT array_agg(pc.chat_id) INTO chats_id
    FROM private.public_chats pc
    WHERE NOT EXISTS (
        SELECT 1
        FROM private.public_chats_members pcm
        WHERE pcm.chat_id = pc.chat_id
          AND pcm.user_id IS NOT NULL);

    chats_count = array_length(chats_id, 1);

    FOREACH deleting_chat_id IN ARRAY chats_id
    LOOP
        chat_id_converted_to_text = replace(deleting_chat_id::text, '-', '_');

        WITH deleted AS (
        DELETE
        FROM private.personal_messages_attachments
        WHERE chat_id = deleting_chat_id
        RETURNING attachment_id)

        UPDATE private.media
        SET links_count = links_count - 1
        WHERE media_id IN (
            SELECT attachment_id
            FROM deleted
        );

        EXECUTE 'DROP TABLE IF EXISTS private.public_messages_attachments_' || chat_id_converted_to_text;
        EXECUTE 'DROP TABLE IF EXISTS private.public_messages_' || chat_id_converted_to_text;
        EXECUTE 'DROP TABLE IF EXISTS private.public_chats_members_' || chat_id_converted_to_text;
        EXECUTE 'DROP TABLE IF EXISTS private.public_chats_audit_logs_' || chat_id_converted_to_text;
        EXECUTE 'DROP TABLE IF EXISTS private.public_chats_banned_users_' || chat_id_converted_to_text;

        DELETE FROM private.media
        WHERE media_id = (
            SELECT avatar
            FROM private.public_chats pc
            WHERE pc.chat_id = deleting_chat_id
            LIMIT 1);

        DELETE FROM private.public_chats
        WHERE public_chats.chat_id = deleting_chat_id;
    END LOOP;

    SELECT array_agg(pc.chat_id) INTO chats_id
    FROM private.personal_chats pc
    WHERE NOT EXISTS (
        SELECT 1
        FROM private.personal_chats_members pcm
        WHERE pcm.chat_id = pc.chat_id
          AND pcm.user_id IS NOT NULL);

    chats_count = chats_count + array_length(chats_id, 1);

    FOREACH deleting_chat_id IN ARRAY chats_id
    LOOP
        chat_id_converted_to_text = replace(deleting_chat_id::text, '-', '_');

        WITH deleted AS (
        DELETE
        FROM private.personal_messages_attachments
        WHERE chat_id = deleting_chat_id
        RETURNING attachment_id)

        UPDATE private.media
        SET links_count = links_count - 1
        WHERE media_id IN (
            SELECT attachment_id
            FROM deleted
        );

        EXECUTE 'DROP TABLE IF EXISTS private.personal_messages_attachments_' || chat_id_converted_to_text;
        EXECUTE 'DROP TABLE IF EXISTS private.personal_messages_' || chat_id_converted_to_text;

        DELETE FROM private.personal_chats
        WHERE personal_chats.chat_id = deleting_chat_id;
    END LOOP;

    SELECT array_agg(bc.chat_id) INTO chats_id
    FROM private.bot_chats bc
    WHERE bc.user_id IS NULL;

    chats_count = chats_count + array_length(chats_id, 1);

    FOREACH deleting_chat_id IN ARRAY chats_id
    LOOP
        chat_id_converted_to_text = replace(deleting_chat_id::text, '-', '_');

        WITH deleted AS (
        DELETE
        FROM private.bot_messages_attachments
        WHERE chat_id = deleting_chat_id
        RETURNING attachment_id)

        UPDATE private.media
        SET links_count = links_count - 1
        WHERE media_id IN (
            SELECT attachment_id
            FROM deleted
        );

        EXECUTE 'DROP TABLE IF EXISTS private.bot_messages_attachments_' || chat_id_converted_to_text;
        EXECUTE 'DROP TABLE IF EXISTS private.bot_messages_' || chat_id_converted_to_text;

        DELETE FROM private.bot_chats
        WHERE bot_chats.chat_id = deleting_chat_id;
    END LOOP;

    RETURN chats_count;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE private.unban_users_by_sentence_time()--every 1 minute
AS
$$
    DELETE
    FROM private.banned_users
    WHERE unbanned_at < current_timestamp;
$$
LANGUAGE sql;

CREATE OR REPLACE PROCEDURE private.delete_expired_refresh_tokens()--every 1 minute
AS
$$
    DELETE
    FROM private.users_refresh_tokens
    WHERE created_at + lifetime <= CURRENT_TIMESTAMP;
$$
LANGUAGE sql;

CREATE OR REPLACE PROCEDURE private.create_log_table_for_current_month()--every 1 month and on starting up
AS
$$
DECLARE
    current_month text = to_char(CURRENT_DATE, 'YYYY-MM');
    values_to text = to_char(CURRENT_DATE + '1 month'::interval, 'YYYY-MM') || '-01';
    administrators_table_name text = 'private.administrators_actions_log_' || replace(current_month, '-', '_');
    administrators_fk_name text = administrators_table_name || '_administrator_id_fkey';
BEGIN
    IF NOT exists(SELECT 1 FROM pg_tables WHERE schemaname = 'private' AND tablename = administrators_table_name) THEN
        EXECUTE 'CREATE TABLE ' || administrators_table_name ||
                ' PARTITION OF private.administrators_actions_log' ||
                ' FOR VALUES FROM (''' || current_month || '-01'') TO (''' || values_to || ''')';

        EXECUTE 'ALTER TABLE ' || administrators_table_name ||
                ' ADD CONSTRAINT ' || replace(administrators_fk_name, 'private.', '') ||
                ' FOREIGN KEY (administrator_id) REFERENCES private.administrators(administrator_id)' ||
                ' ON DELETE CASCADE ON UPDATE CASCADE';
    END IF;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE private.delete_log_tables(expiration_months dom_positive_int)--every 1 month and on starting up
AS
$$
DECLARE
    deleting_tables text[];
    deleting_table_name text;
BEGIN
    SELECT array_agg(table_name) INTO deleting_tables
    FROM information_schema.tables
    WHERE table_catalog = 'messenger_db'
      AND table_schema = 'private'
      AND table_name LIKE 'administrators\_actions\_log\_%' ESCAPE '\'
      AND (to_date(replace(table_name, 'administrators_actions_log_', ''), 'YYYY-MM') + (expiration_months || ' month')::interval) <= now();

    FOREACH deleting_table_name IN ARRAY deleting_tables LOOP
        EXECUTE 'DROP TABLE IF EXISTS private.' || deleting_table_name;
    END LOOP;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE private.log_administrator_action(administrator_id int, action varchar(128))
AS
$$
    INSERT INTO private.administrators_actions_log (action_datetime, administrator_id, action)
    VALUES (CURRENT_TIMESTAMP, log_administrator_action.administrator_id, log_administrator_action.action);
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION private.update_user_online_status(user_id uuid)
RETURNS void
AS
$$
    UPDATE private.users
    SET was_online = CURRENT_TIMESTAMP
    WHERE user_id = update_user_online_status.user_id;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION private.get_message(chat_id uuid, message_id uuid, getting_by uuid, chat_type en_chat_type)
RETURNS message
AS
$$
DECLARE
    returning_message message;
BEGIN
    IF chat_type = 'Personal' THEN
        SELECT pm.message_id, author, message_text, sent_at, is_updated, updated_at, pm.reply_to, pm.resent_from, pm.is_bot_resend,
           coalesce(array_agg(pma.attachment_id) FILTER (WHERE attachment_id IS NOT NULL), '{}') INTO returning_message
        FROM private.personal_messages pm
        LEFT JOIN private.personal_messages_attachments pma ON pm.chat_id = pma.chat_id AND pm.message_id = pma.message_id
        WHERE pm.chat_id = get_message.chat_id
          AND exists(
            SELECT 1
            FROM private.personal_chats_members pcm
            WHERE pcm.chat_id = get_message.chat_id
          AND pcm.user_id = getting_by)
          AND pm.message_id = get_message.message_id
        GROUP BY pm.message_id, author, message_text, sent_at, is_updated, updated_at, pm.reply_to, pm.resent_from, pm.is_bot_resend;
    ELSIF chat_type = 'Public' THEN
        SELECT pm.message_id, author, message_text, sent_at, is_updated, updated_at, pm.reply_to, pm.resent_from, pm.is_bot_resend,
           coalesce(array_agg(pma.attachment_id) FILTER (WHERE attachment_id IS NOT NULL), '{}') INTO returning_message
        FROM private.public_messages pm
        LEFT JOIN private.public_messages_attachments pma ON pm.chat_id = pma.chat_id AND pm.message_id = pma.message_id
        WHERE pm.chat_id = get_message.chat_id
          AND exists(
            SELECT 1
            FROM private.public_chats_members pcm
            WHERE pcm.chat_id = get_message.chat_id
              AND pcm.user_id = getting_by)
          AND pm.message_id = get_message.message_id
        GROUP BY pm.message_id, author, message_text, sent_at, is_updated, updated_at, pm.reply_to, pm.resent_from, pm.is_bot_resend;
    ELSIF chat_type = 'Bot' THEN
        SELECT bm.message_id, CASE WHEN bm.is_bot THEN bc.bot_id ELSE bc.user_id END AS author,
        bm.message_text, bm.sent_at, bm.is_updated, bm.updated_at, bm.reply_to, bm.resent_from, bm.is_bot_resend,
        coalesce(att.attached_media, '{}') INTO returning_message
        FROM private.bot_messages bm
        JOIN private.bot_chats bc ON bc.chat_id = bm.chat_id
        LEFT JOIN LATERAL (
            SELECT array_agg(bma.attachment_id) AS attached_media
            FROM private.bot_messages_attachments bma
            WHERE bma.chat_id = bm.chat_id AND bma.message_id = bm.message_id
        ) att ON true
        WHERE bm.chat_id = get_message.chat_id AND bc.user_id = get_message.getting_by AND bm.message_id = get_message.message_id;
    ELSE
        RAISE DATA_EXCEPTION;
    END IF;

    RETURN returning_message;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION private.register_administrator(administrator_login dom_auth_string, administrator_password dom_auth_string)
RETURNS int
AS
$$
    INSERT INTO private.administrators (login_hash, password_hash) VALUES
    (sha256(administrator_login::bytea), sha256(administrator_password::bytea))
    RETURNING administrator_id;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION private.delete_administrator(administrator_id int)
RETURNS int
AS
$$
DECLARE
    affected_rows int;
BEGIN
    DELETE FROM private.administrators
    WHERE private.administrators.administrator_id = delete_administrator.administrator_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION private.tgr_delete_empty_private_messages()
RETURNS TRIGGER
AS
$$
BEGIN
    IF (SELECT count(*)
        FROM private.personal_messages_attachments
        WHERE chat_id = OLD.chat_id
          AND message_id = OLD.message_id)
        = 0 THEN

        DELETE FROM private.personal_messages
        WHERE chat_id = OLD.chat_id
          AND message_id = OLD.message_id
          AND message_text = '';

    END IF;
    RETURN OLD;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER delete_empty_private_message_after_delete_private_attachment
AFTER DELETE ON private.personal_messages_attachments
FOR EACH ROW
EXECUTE FUNCTION private.tgr_delete_empty_private_messages();

CREATE OR REPLACE FUNCTION private.tgr_delete_empty_public_messages()
RETURNS TRIGGER
AS
$$
BEGIN
    IF (SELECT count(*)
        FROM private.public_messages_attachments
        WHERE chat_id = OLD.chat_id
          AND message_id = OLD.message_id)
        = 0 THEN

        DELETE FROM private.public_messages
        WHERE chat_id = OLD.chat_id
          AND message_id = OLD.message_id
          AND message_text = '';

    END IF;
    RETURN OLD;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER delete_empty_public_message_after_delete_public_attachment
AFTER DELETE ON private.public_messages_attachments
FOR EACH ROW
EXECUTE FUNCTION private.tgr_delete_empty_public_messages();

CREATE OR REPLACE FUNCTION private.tgr_delete_detached_media()
RETURNS TRIGGER
AS
$$
BEGIN
    DELETE
    FROM private.media
    WHERE media_id = NEW.media_id
      AND links_count <= 0;

    RETURN NULL;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER delete_detached_media_after_update_media
AFTER UPDATE OF links_count ON private.media
FOR EACH ROW
WHEN (NEW.links_count <= 0)
EXECUTE FUNCTION private.tgr_delete_detached_media();

CREATE OR REPLACE FUNCTION private.tgr_save_deleted_media_id()
RETURNS TRIGGER
AS
$$
BEGIN
    INSERT INTO private.deleted_media_list
    VALUES (OLD.media_id);

    RETURN OLD;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER save_deleted_media_id_after_delete_media
AFTER DELETE ON private.media
FOR EACH ROW
EXECUTE FUNCTION private.tgr_save_deleted_media_id();

CREATE OR REPLACE FUNCTION private.tgr_check_message_report()
RETURNS TRIGGER
AS
$$
DECLARE
    is_consistent bool;
BEGIN
    CASE NEW.chat_type
        WHEN 'Personal' THEN
            SELECT exists(
                SELECT 1
                FROM private.personal_messages
                WHERE chat_id = NEW.chat_id
                  AND message_id = NEW.message_id)
            INTO is_consistent;
        WHEN 'Public' THEN
            SELECT exists(
                SELECT 1
                FROM private.public_messages
                WHERE chat_id = NEW.chat_id
                  AND message_id = NEW.message_id)
            INTO is_consistent;
        WHEN 'Bot' THEN
            SELECT exists(
                SELECT 1
                FROM private.bot_messages
                WHERE chat_id = NEW.chat_id
                  AND message_id = NEW.message_id)
            INTO is_consistent;
        ELSE
            is_consistent = false;
        END CASE;

    IF NOT is_consistent THEN
        RAISE EXCEPTION 'Message does not exist'
            USING ERRCODE = '23514';
    END IF;

    RETURN NEW;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER check_message_report_before_insert_report
BEFORE INSERT ON private.users_message_reports
FOR EACH ROW
EXECUTE FUNCTION private.tgr_check_message_report();


CREATE OR REPLACE FUNCTION sch_user.validate_refresh_token(token varchar(44), device_id uuid, user_id uuid)
RETURNS bool
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
BEGIN
    IF exists(
        SELECT 1
        FROM private.users_refresh_tokens urt
        WHERE token = refresh_token
          AND urt.device_id = validate_refresh_token.device_id
          AND urt.user_id = validate_refresh_token.user_id
    ) THEN
        RETURN true;
    ELSE
        DELETE
        FROM private.users_refresh_tokens urt
        WHERE urt.refresh_token = token
           OR urt.user_id = validate_refresh_token.user_id
           OR urt.device_id = validate_refresh_token.device_id;

        RETURN false;
    END IF;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.create_refresh_token(user_id uuid, lifetime interval, device_id uuid)
RETURNS varchar(44)
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    INSERT INTO private.users_refresh_tokens (refresh_token, user_id, device_id, lifetime)
    VALUES (encode(sha256((create_refresh_token.user_id :: text || create_refresh_token.device_id :: text || gen_random_uuid() :: text) :: bytea), 'base64'),
            create_refresh_token.user_id, create_refresh_token.device_id, create_refresh_token.lifetime)
    RETURNING refresh_token
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.update_refresh_token(user_id uuid, old_token varchar(44), device_id uuid)
RETURNS varchar(44)
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
BEGIN
    IF NOT validate_refresh_token(old_token, device_id, user_id) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    UPDATE private.users_refresh_tokens
    SET created_at = CURRENT_TIMESTAMP,
        refresh_token = encode(sha256((update_refresh_token.user_id :: text || update_refresh_token.device_id :: text || gen_random_uuid() :: text) :: bytea), 'base64')
    WHERE refresh_token = old_token
    RETURNING refresh_token;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.invalidate_refresh_token(token varchar(44))
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    DELETE
    FROM users_refresh_tokens
    WHERE refresh_token = token;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;




CREATE OR REPLACE FUNCTION sch_user.report_message(reported_by uuid, chat_type en_chat_type, chat_id uuid, message_id uuid, comment text)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    INSERT INTO private.users_message_reports (reported_by, chat_type, chat_id, message_id, comment) VALUES
        (report_message.reported_by, report_message.chat_type,
         report_message.chat_id, report_message.message_id, report_message.comment);

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.report_user(reported_by uuid, reported_user_id uuid, comment text)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    INSERT INTO private.users_user_reports (reported_by, reported_user_id, comment) VALUES
        (report_user.reported_by, report_user.reported_user_id, report_user.comment);

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.report_bot(reported_by uuid, bot_id uuid, comment text)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    INSERT INTO private.users_bot_reports (reported_by, bot_id, comment) VALUES
        (report_bot.reported_by, report_bot.bot_id, report_bot.comment);

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.report_public_chat(reported_by uuid, chat_id uuid, comment text)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    INSERT INTO private.users_public_chat_reports (reported_by, chat_id, comment) VALUES
        (report_public_chat.reported_by, report_public_chat.chat_id, report_public_chat.comment);

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.report_administrator(reported_by uuid, administrator_id int, comment text)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    INSERT INTO private.users_administration_reports (reported_by, administrator_id, comment) VALUES
        (report_administrator.reported_by, report_administrator.administrator_id, report_administrator.comment);

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;




CREATE OR REPLACE FUNCTION sch_user.register_user(user_login dom_auth_string, user_password dom_auth_string, first_name varchar(32),
                              last_name varchar(32), tag varchar(16), birth_date timestamp)
RETURNS uuid
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    new_user_id uuid = gen_random_uuid();
    user_id_text text = replace(new_user_id::text, '-', '_');
    avatars_table_name text = 'private.avatars_' || user_id_text;
    users_fk_name text = 'avatars_users_' || user_id_text || '_user_id_fkey';
    media_fk_name text = 'avatars_media_' || user_id_text || '_media_id_fkey';
    salty_user_password text = user_password || user_id_text;
BEGIN
    INSERT INTO private.users (login_hash, password_hash, first_name, last_name, tag, birth_date, was_online, user_id) VALUES
        (sha256(user_login::bytea), sha256(salty_user_password::bytea),
            register_user.first_name, register_user.last_name, register_user.tag, register_user.birth_date, CURRENT_TIMESTAMP, new_user_id);

    EXECUTE 'CREATE TABLE ' || avatars_table_name ||
            ' PARTITION OF private.avatars FOR VALUES IN (''' ||
            new_user_id::text || ''')';

    EXECUTE 'ALTER TABLE ' || avatars_table_name ||
            ' ADD CONSTRAINT ' || users_fk_name ||
            ' FOREIGN KEY (user_id) REFERENCES private.users(user_id)' ||
            ' ON DELETE CASCADE ON UPDATE CASCADE';

    EXECUTE 'ALTER TABLE ' || avatars_table_name ||
            ' ADD CONSTRAINT ' || media_fk_name ||
            ' FOREIGN KEY (media_id) REFERENCES private.media(media_id)' ||
            ' ON DELETE CASCADE ON UPDATE CASCADE';

    RETURN new_user_id;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.auth_user(user_login text, user_password text)
RETURNS user_data
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    user_data user_data;
    password_hash bytea;
BEGIN
    SELECT ROW(user_id, first_name, last_name, tag, avatar, birth_date, bio, was_online)::user_data INTO user_data
    FROM private.users
    WHERE login_hash = sha256(user_login::bytea);

    SELECT password_hash INTO password_hash
    FROM private.users
    WHERE login_hash = sha256(user_login::bytea);

    IF user_data IS NULL THEN
        RETURN null;
    END IF;

    IF password_hash != sha256((user_password || replace(user_data.user_id :: text, '-', '_'))::bytea) THEN
        RETURN null;
    END IF;
    RETURN user_data;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.get_user_by_id(user_id uuid)
RETURNS user_data
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT ROW(users.user_id, first_name, last_name, tag, avatar, birth_date, bio, was_online) :: user_data
    FROM private.users
    WHERE users.user_id = get_user_by_id.user_id;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.check_user_ban_status(user_id uuid)
RETURNS boolean
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    is_banned boolean;
BEGIN
    SELECT true into is_banned
    FROM private.banned_users
    WHERE banned_users.user_id = check_user_ban_status.user_id;
    RETURN coalesce(is_banned, false);
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.get_banned_user_information(user_id uuid)
RETURNS private.banned_users
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT *
    FROM private.banned_users
    WHERE banned_users.user_id = get_banned_user_information.user_id;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_user_by_tag(tag varchar(16))
RETURNS user_data
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT ROW(user_id, first_name, last_name, users.tag, avatar, birth_date, bio, was_online) :: user_data
    FROM private.users
    WHERE users.tag = get_user_by_tag.tag;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_user_avatars(user_id uuid)
RETURNS SETOF uuid
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT media_id
    FROM private.avatars
    WHERE avatars.user_id = get_user_avatars.user_id;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_user_chats(user_id uuid, page dom_positive_int, page_size dom_positive_int)--profile
RETURNS SETOF chat_information
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
SELECT private.update_user_online_status(user_id);

SELECT chat_id, chat_name, coalesce(new_messages_count, 0), chat_image, chat_type
FROM (
    SELECT pcm1.chat_id chat_id, coalesce(concat(users.first_name, ' ', coalesce(users.last_name, '')), 'Deleted user') chat_name, nmc.new_messages_count, users.avatar chat_image, 'Personal' :: en_chat_type chat_type, lmsa.sent_at
    FROM private.personal_chats_members pcm1
    LEFT JOIN private.personal_chats_members pcm2 ON pcm1.chat_id = pcm2.chat_id AND pcm1.user_id != pcm2.user_id
    LEFT JOIN private.users ON pcm2.user_id = users.user_id  --информация о пользователях-адресатах
    JOIN (
        SELECT DISTINCT ON (chat_id) chat_id, sent_at
        FROM private.personal_messages
        ORDER BY chat_id, sent_at DESC) lmsa ON lmsa.chat_id = pcm1.chat_id    --время отправки последнего сообщения в чате
    LEFT JOIN (
        SELECT count(message_id) new_messages_count, personal_chats_members.chat_id
        FROM private.personal_messages
        JOIN private.personal_chats_members ON personal_messages.chat_id = personal_chats_members.chat_id AND author = personal_chats_members.user_id
        WHERE sent_at > was_in_chat AND personal_messages.author != get_user_chats.user_id
        GROUP BY personal_chats_members.chat_id) nmc ON pcm1.chat_id = nmc.chat_id --количество новых сообщений в чатах
    WHERE pcm1.user_id = get_user_chats.user_id

    UNION ALL

    SELECT chats_with_user.chat_id chat_id, chats_with_user.chat_name chat_name, nmc.new_messages_count, chats_with_user.avatar chat_image, 'Public' :: en_chat_type chat_type, lmsa.sent_at
    FROM (
        SELECT public_chats.*
        FROM private.public_chats
        JOIN private.public_chats_members ON public_chats.chat_id = public_chats_members.chat_id
        WHERE get_user_chats.user_id = public_chats_members.user_id) chats_with_user    --чаты с текущим пользователем
    JOIN (
        SELECT DISTINCT ON (chat_id) chat_id, sent_at
        FROM private.public_messages
        ORDER BY chat_id, sent_at DESC) lmsa ON lmsa.chat_id = chats_with_user.chat_id    --время отправки последнего сообщения в чате
    LEFT JOIN (
        SELECT count(message_id) new_messages_count, public_chats_members.chat_id
        FROM private.public_messages
        JOIN private.public_chats_members ON public_messages.chat_id = public_chats_members.chat_id AND author = public_chats_members.user_id
        WHERE sent_at > was_in_chat AND author != get_user_chats.user_id
        GROUP BY public_chats_members.chat_id) nmc ON chats_with_user.chat_id = nmc.chat_id --количество новых сообщений в чатах

    UNION ALL

    SELECT bc.chat_id, coalesce(b.name, 'Deleted bot') AS chat_name, nmc.new_messages_count, b.avatar AS chat_image, 'Bot'::en_chat_type AS chat_type, lmsa.sent_at
    FROM private.bot_chats bc
    LEFT JOIN private.bots b ON b.bot_id = bc.bot_id
    JOIN (
        SELECT DISTINCT ON (chat_id) chat_id, sent_at
        FROM private.bot_messages
        ORDER BY chat_id, sent_at DESC) lmsa ON lmsa.chat_id = bc.chat_id
    LEFT JOIN (
        SELECT count(bm.message_id) AS new_messages_count, bc2.chat_id
        FROM private.bot_messages bm
        JOIN private.bot_chats bc2 ON bm.chat_id = bc2.chat_id
        WHERE bm.sent_at > bc2.was_in_chat
        GROUP BY bc2.chat_id) nmc ON bc.chat_id = nmc.chat_id
    WHERE bc.user_id = get_user_chats.user_id
    ) unsorted_result
ORDER BY sent_at DESC
OFFSET (page - 1) * page_size
LIMIT page_size;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.update_user_auth(user_id uuid, user_current_password text,
                                  user_new_login dom_auth_string DEFAULT NULL, user_new_password dom_auth_string DEFAULT NULL)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
    new_salty_user_password text;
BEGIN
    SELECT private.update_user_online_status(user_id);

    IF user_new_password IS NOT NULL THEN
        new_salty_user_password = user_new_password || replace(user_id, '-', '_');
    END IF;

    UPDATE private.users
    SET login_hash = coalesce(sha256(user_new_login::bytea), login_hash),
        password_hash = coalesce(sha256(new_salty_user_password::bytea), password_hash)
    WHERE user_id = update_user_auth.user_id
      AND password_hash = sha256((user_current_password || replace(user_id, '-', '_'))::bytea);

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.update_user_data(new_user_data user_data)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(new_user_data.user_id);

    UPDATE private.users
    SET first_name = coalesce(new_user_data.first_name, first_name),
        last_name = coalesce(new_user_data.last_name, last_name),
        tag = coalesce(new_user_data.tag, tag),
        birth_date = coalesce(new_user_data.birth_date, birth_date),
        bio = coalesce(new_user_data.bio, bio)
    WHERE user_id = new_user_data.user_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.update_user_avatar(user_id uuid, new_user_avatar media_file)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(user_id);

    INSERT INTO private.media (media_id, file_name, content_type)
    VALUES (new_user_avatar.media_id, new_user_avatar.file_name, new_user_avatar.content_type);

    INSERT INTO private.avatars (media_id, user_id)
    VALUES (new_user_avatar.media_id, user_id);

    UPDATE private.users
    SET avatar = new_user_avatar.media_id
    WHERE user_id = update_user_avatar.user_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.delete_user_avatar(user_id uuid, avatar uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(user_id);

    DELETE FROM private.media
    WHERE media_id = delete_user_avatar.avatar;

    UPDATE private.users
    SET avatar = (
        SELECT media_id
        FROM private.avatars
        WHERE avatars.user_id = delete_user_avatar.user_id
        ORDER BY uploaded_at DESC
        LIMIT 1
        )
    WHERE user_id = delete_user_avatar.user_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.delete_user(user_id uuid, user_password text)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE

    user_id_text text = replace(user_id::text, '-', '_');
    avatars_table_name text = 'private.avatars_' || user_id_text;
    affected_rows int;
BEGIN
    DELETE FROM private.media
    WHERE media_id IN (SELECT media_id FROM private.avatars WHERE avatars.user_id = delete_user.user_id);

    EXECUTE 'DROP TABLE IF EXISTS ' || avatars_table_name;

    DELETE FROM private.users
    WHERE users.user_id = delete_user.user_id
      AND password_hash = sha256((user_password :: text || replace(delete_user.user_id :: text, '-', '_')) :: bytea);

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION sch_user.get_bot_info(bot_id uuid)
RETURNS bot_info
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT b.bot_id, name, tag, avatar, description, is_enabled
    FROM private.bots b
    WHERE b.bot_id = get_bot_info.bot_id;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_bot_by_name(name varchar(32))
RETURNS bot_info
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT bot_id, b.name, tag, avatar, description, is_enabled
    FROM private.bots b
    WHERE b.name LIKE '%' || get_bot_by_name.name || '%';
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_bot_by_tag(tag varchar(16))
RETURNS bot_info
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT bot_id, name, b.tag, avatar, description, is_enabled
    FROM private.bots b
    WHERE b.tag = get_bot_by_tag.tag ;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.list_bots(getting_by uuid)
RETURNS SETOF bot_info
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    SELECT bot_id, name, tag, avatar, description, is_enabled
    FROM private.bots
    WHERE owner = getting_by;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_bot_token(bot_id uuid, getting_by uuid)
RETURNS bot_token_info
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    SELECT token_hash, token_version
    FROM private.bots b
    WHERE b.bot_id = get_bot_token.bot_id
      AND owner = getting_by
    LIMIT 1;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.list_commands(bot_id uuid)
RETURNS SETOF bot_command_info
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT bc.command_id, bc.prefix, bc.command, bc.description,
           coalesce(array_agg(ROW(bca.argument_id, bca.name, bca.type) :: bot_command_argument ORDER BY bca.argument_id) FILTER (WHERE argument_id IS NOT NULL), '{}')
    FROM private.bots_commands bc
    LEFT JOIN private.bots_commands_arguments bca on bc.bot_id = bca.bot_id and bc.command_id = bca.command_id
    WHERE bc.bot_id = list_commands.bot_id
    GROUP BY bc.command_id, bc.prefix, bc.command, bc.description
    ORDER BY prefix, command;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.list_bot_connections(bot_id uuid, getting_by uuid)
RETURNS SETOF bot_connection_log_record
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    SELECT ip_address, connected_at, bcl.token_version
    FROM private.bots_connections_logs bcl
    JOIN private.bots b on b.bot_id = bcl.bot_id
    WHERE bcl.bot_id = list_bot_connections.bot_id
      AND b.owner = getting_by
    ORDER BY connected_at DESC
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.create_bot(name varchar(32), tag varchar(16), owner uuid, avatar media_file DEFAULT NULL, description varchar(512) DEFAULT NULL)
RETURNS uuid
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    new_bot_id uuid = gen_random_uuid();
BEGIN
    SELECT private.update_user_online_status(owner);

    IF avatar IS NOT NULL THEN
        INSERT INTO private.media (media_id, file_name, content_type)
        VALUES (avatar.media_id, avatar.file_name, avatar.content_type);
    END IF;

    INSERT INTO private.bots (bot_id, token_hash, token_version, name, tag, avatar, description, owner, is_enabled)
    VALUES (new_bot_id, sha256((new_bot_id :: text || create_bot.owner :: text || gen_random_uuid() :: text) :: bytea),
            1, create_bot.name, create_bot.tag, create_bot.avatar.media_id,
            create_bot.description, create_bot.owner, true);

    RETURN new_bot_id;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.add_command(bot_id uuid, adding_by uuid, prefix char, command varchar(8), description varchar(32) DEFAULT NULL)
RETURNS dom_positive_int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    last_command_id int;
BEGIN
    SELECT private.update_user_online_status(adding_by);

    IF NOT exists(
        SELECT 1
        FROM private.bots b
        WHERE b.bot_id = add_command.bot_id
          AND owner = adding_by
    ) THEN
        RAISE NO_DATA_FOUND;
    END IF;

    SELECT coalesce(max(command_id), 0) INTO last_command_id
    FROM private.bots_commands bc
    WHERE bc.bot_id = add_command.bot_id;

    INSERT INTO private.bots_commands (bot_id, command_id, prefix, command, description)
    VALUES (add_command.bot_id, last_command_id + 1,
            add_command.prefix, add_command.command, add_command.description);

    RETURN last_command_id + 1;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.add_command_argument(bot_id uuid, adding_by uuid, command_id dom_positive_int, name varchar(32), type varchar(32))
RETURNS dom_positive_int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    last_argument_id int;
BEGIN
    SELECT private.update_user_online_status(adding_by);

    IF NOT exists(
        SELECT 1
        FROM private.bots b
        JOIN private.bots_commands bc on b.bot_id = bc.bot_id
        WHERE b.bot_id = add_command_argument.bot_id
          AND owner = adding_by
          AND bc.command_id = add_command_argument.command_id
    ) THEN
        RAISE NO_DATA_FOUND;
    END IF;

    SELECT coalesce(max(argument_id), 0) INTO last_argument_id
    FROM private.bots_commands_arguments bca
    WHERE bca.bot_id = add_command_argument.bot_id
      AND bca.command_id = add_command_argument.command_id;

    INSERT INTO private.bots_commands_arguments (bot_id, command_id, argument_id, name, type)
    VALUES (add_command_argument.bot_id, add_command_argument.command_id, last_argument_id + 1,
            add_command_argument.name, add_command_argument.type);

    RETURN last_argument_id + 1;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.update_bot(bot_id uuid, updating_by uuid, update_avatar bool, update_description bool, name varchar(32) DEFAULT NULL, tag varchar(16) DEFAULT NULL, avatar media_file DEFAULT NULL, description varchar(512) DEFAULT NULL)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    old_avatar uuid;
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(updating_by);

    IF avatar IS NOT NULL AND update_avatar THEN
        INSERT INTO private.media (media_id, file_name, content_type)
        VALUES (avatar.media_id, avatar.file_name, avatar.content_type);
    END IF;

    UPDATE private.bots
    SET name = coalesce(update_bot.name, name),
        tag = coalesce(update_bot.tag, tag),
        description = coalesce(update_bot.description, description),--
        avatar = coalesce(update_bot.avatar.media_id, avatar)--
    WHERE bot_id = update_bot.bot_id
      AND owner = updating_by;

    IF update_avatar THEN
        SELECT b.avatar INTO old_avatar
        FROM private.bots b
            WHERE b.bot_id = update_bot.bot_id
              AND owner = updating_by;

        UPDATE private.bots
        SET avatar = update_bot.avatar.media_id
        WHERE bot_id = update_bot.bot_id
          AND owner = updating_by;

        IF old_avatar IS NOT NULL THEN
            DELETE
            FROM media
            WHERE media_id = old_avatar;
        END IF;
    END IF;

    IF update_description THEN
        UPDATE private.bots
        SET description = update_bot.description
        WHERE bot_id = update_bot.bot_id
          AND owner = updating_by;
    END IF;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.regenerate_bot_token(bot_id uuid, updating_by uuid)
RETURNS bot_token_info
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(updating_by);

    UPDATE private.bots b
    SET token_hash = sha256((bot_id::text || owner :: text || gen_random_uuid() :: text) :: bytea),
        token_version = token_version + 1
    WHERE b.bot_id = regenerate_bot_token.bot_id
      AND owner = updating_by
    RETURNING token_hash, token_version;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.update_command(bot_id uuid, updating_by uuid, command_id dom_positive_int, new_prefix char DEFAULT NULL, new_command varchar(8) DEFAULT NULL, new_description varchar(32) DEFAULT NULL)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(updating_by);

    IF NOT exists(
        SELECT 1
        FROM private.bots b
        WHERE b.bot_id = update_command.bot_id
          AND owner = updating_by
    ) THEN
        RAISE NO_DATA_FOUND;
    END IF;

    UPDATE private.bots_commands bc
    SET prefix = coalesce(new_prefix, prefix),
        command = coalesce(new_command, command),
        description = coalesce(new_description, description)
    WHERE bc.bot_id = update_command.bot_id
      AND bc.command_id = update_command.command_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.update_command_argument(bot_id uuid, updating_by uuid, command_id dom_positive_int, argument_id dom_positive_int, new_name varchar(32) DEFAULT NULL, new_type varchar(32) DEFAULT NULL)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(updating_by);

    IF NOT exists(
        SELECT 1
        FROM private.bots b
        WHERE b.bot_id = update_command_argument.bot_id
          AND owner = updating_by
    ) THEN
        RAISE NO_DATA_FOUND;
    END IF;

    UPDATE private.bots_commands_arguments bca
    SET name = coalesce(new_name, name),
        type = coalesce(new_type, type)
    WHERE bca.bot_id = update_command_argument.bot_id
      AND bca.argument_id = update_command_argument.argument_id
      AND bca.command_id = update_command_argument.command_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.delete_bot(bot_id uuid, deleting_by uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
    avatar_id uuid;
BEGIN
    SELECT private.update_user_online_status(deleting_by);

    IF NOT exists(
        SELECT 1
        FROM private.bots b
        WHERE b.bot_id = delete_bot.bot_id
          AND owner = deleting_by
    ) THEN
        RAISE NO_DATA_FOUND;
    END IF;

    SELECT b.avatar INTO avatar_id
    FROM bots b
    WHERE b.bot_id = delete_bot.bot_id;

    IF avatar_id IS NOT NULL THEN
        DELETE
        FROM media
        WHERE media_id = avatar_id;
    END IF;

    DELETE
    FROM bots b
    WHERE b.bot_id = delete_bot.bot_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.delete_command(bot_id uuid, deleting_by uuid, command_id dom_positive_int)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(deleting_by);

    IF NOT exists(
        SELECT 1
        FROM private.bots b
        WHERE b.bot_id = delete_command.bot_id
          AND owner = deleting_by
    ) THEN
        RAISE NO_DATA_FOUND;
    END IF;

    DELETE
    FROM private.bots_commands bc
    WHERE bc.bot_id = delete_command.bot_id
      AND bc.command_id = delete_command.command_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    UPDATE bots_commands bc
    SET command_id = bc.command_id - 1
    WHERE bc.bot_id = delete_command.bot_id
      AND bc.command_id > delete_command.command_id;

    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.delete_command_argument(bot_id uuid, deleting_by uuid, command_id dom_positive_int, argument_id dom_positive_int)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(deleting_by);

    IF NOT exists(
        SELECT 1
        FROM bots b
        WHERE b.bot_id = delete_command_argument.bot_id
          AND owner = deleting_by
    ) THEN
        RAISE NO_DATA_FOUND;
    END IF;

    DELETE
    FROM bots_commands_arguments bca
    WHERE bca.bot_id = delete_command_argument.bot_id
      AND bca.command_id = delete_command_argument.command_id
      AND bca.argument_id = delete_command_argument.argument_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    UPDATE bots_commands_arguments bca
    SET argument_id = bca.argument_id - 1
    WHERE bca.bot_id = delete_command_argument.bot_id
      AND bca.command_id = delete_command_argument.command_id
      AND bca.argument_id > delete_command_argument.argument_id;

    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;





CREATE OR REPLACE FUNCTION sch_user.get_personal_chat_short_info(chat_id uuid, user_id uuid)
RETURNS chat_information
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(user_id);

    SELECT pcm.chat_id, coalesce(u.first_name || ' ' || u.last_name, 'Deleted user'), coalesce(nmc.new_messages_count, 0), u.avatar, 'Personal' :: en_chat_type
    FROM private.personal_chats_members pcm
    LEFT JOIN private.users u ON u.user_id = pcm.user_id
    LEFT JOIN (
        SELECT count(pm.message_id) AS new_messages_count, pm.chat_id
        FROM private.personal_messages pm
        JOIN private.personal_chats_members pcm_self
            ON pm.chat_id = pcm_self.chat_id
        WHERE pm.sent_at > pcm_self.was_in_chat
          AND pcm_self.user_id = get_personal_chat_short_info.user_id
          AND pm.author != get_personal_chat_short_info.user_id
          AND pm.chat_id = get_personal_chat_short_info.chat_id
        GROUP BY pm.chat_id) nmc ON nmc.chat_id = pcm.chat_id
    WHERE pcm.chat_id = get_personal_chat_short_info.chat_id
      AND pcm.user_id != get_personal_chat_short_info.user_id;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_messages_from_personal_chat
    (chat_id uuid, getting_by uuid, messages_count dom_positive_int, sent_before timestamp DEFAULT CURRENT_TIMESTAMP)
RETURNS SETOF message
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    UPDATE private.personal_chats_members
    SET was_in_chat = CURRENT_TIMESTAMP
    WHERE chat_id = get_messages_from_personal_chat.chat_id
      AND user_id = getting_by;

    SELECT pm.message_id, author, message_text, sent_at, is_updated, updated_at, pm.reply_to, pm.resent_from, pm.is_bot_resend,
           coalesce(array_agg(pma.attachment_id) FILTER (WHERE attachment_id IS NOT NULL), '{}')
    FROM private.personal_messages pm
    LEFT JOIN private.personal_messages_attachments pma ON pm.chat_id = pma.chat_id AND pm.message_id = pma.message_id
    WHERE pm.chat_id = get_messages_from_personal_chat.chat_id
      AND exists(
        SELECT 1
        FROM private.personal_chats_members pcm
        WHERE pcm.chat_id = get_messages_from_personal_chat.chat_id
          AND pcm.user_id = getting_by)
      AND sent_at < coalesce(sent_before, CURRENT_TIMESTAMP)
    GROUP BY pm.message_id, author, message_text, sent_at, is_updated, updated_at, pm.reply_to, pm.resent_from, pm.is_bot_resend, pm.sent_at
    ORDER BY pm.sent_at DESC
    LIMIT messages_count
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_personal_message(chat_id uuid, message_id uuid, getting_by uuid)
RETURNS message
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    UPDATE private.personal_chats_members
    SET was_in_chat = CURRENT_TIMESTAMP
    WHERE chat_id = get_personal_message.chat_id
      AND user_id = getting_by;

    SELECT pm.message_id, author, message_text, sent_at, is_updated, updated_at, pm.reply_to, pm.resent_from, pm.is_bot_resend,
           coalesce(array_agg(pma.attachment_id) FILTER (WHERE attachment_id IS NOT NULL), '{}')
    FROM private.personal_messages pm
    LEFT JOIN private.personal_messages_attachments pma ON pm.chat_id = pma.chat_id AND pm.message_id = pma.message_id
    WHERE pm.chat_id = get_personal_message.chat_id
      AND exists(
        SELECT 1
        FROM private.personal_chats_members pcm
        WHERE pcm.chat_id = get_personal_message.chat_id
          AND pcm.user_id = getting_by)
      AND pm.message_id = get_personal_message.message_id
    GROUP BY pm.message_id, author, message_text, sent_at, is_updated, updated_at, pm.reply_to, pm.resent_from, pm.is_bot_resend
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_personal_chat_destination_user_id(chat_id uuid, getting_by uuid)
RETURNS uuid
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);
    SELECT pcm2.user_id
    FROM private.personal_chats_members pcm1
    JOIN private.personal_chats_members pcm2 ON pcm1.chat_id = pcm2.chat_id AND pcm1.user_id != pcm2.user_id
    WHERE pcm1.chat_id = get_personal_chat_destination_user_id.chat_id AND pcm1.user_id = getting_by;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.initialize_new_personal_chat(first_owner uuid, second_owner uuid)
RETURNS uuid
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    existing_chat_id uuid;
    initializing_chat_id uuid = gen_random_uuid();
    chat_id_converted_to_text text = replace(initializing_chat_id::text, '-', '_');
    messages_table_name text = 'private.personal_messages_' || chat_id_converted_to_text;
    messages_chat_id_fk_name text = 'personal_messages_' || chat_id_converted_to_text || '_chat_id_fkey';
    messages_reply_to_fk_name text = 'personal_messages_' || chat_id_converted_to_text || '_reply_to_fkey';
    attachments_table_name text = 'private.personal_messages_attachments_' || chat_id_converted_to_text;
    attachments_chat_id_message_id_fk_name text = 'personal_messages_attachments_' || chat_id_converted_to_text || '_chat_id_message_id_fkey';
    attachments_attachment_id_fk_name text = 'personal_messages_attachments_' || chat_id_converted_to_text || '_attachment_id_fkey';
BEGIN
    SELECT private.update_user_online_status(first_owner);

    SELECT pcm1.chat_id INTO existing_chat_id
    FROM private.personal_chats_members pcm1
    JOIN private.personal_chats_members pcm2 ON pcm1.chat_id = pcm2.chat_id
                                    AND pcm1.user_id != pcm2.user_id
    WHERE pcm1.user_id = first_owner AND pcm2.user_id = second_owner
    GROUP BY pcm1.chat_id;

    IF existing_chat_id IS NOT NULL THEN
        RETURN existing_chat_id;
    END IF;

    INSERT INTO private.personal_chats VALUES (initializing_chat_id);

    INSERT INTO private.personal_chats_members (chat_id, user_id) VALUES
            (initializing_chat_id, first_owner),
            (initializing_chat_id, second_owner);

    EXECUTE 'CREATE TABLE ' || messages_table_name ||
            ' PARTITION OF private.personal_messages FOR VALUES IN (''' ||
            initializing_chat_id::text || ''')';

    EXECUTE 'ALTER TABLE ' || messages_table_name ||
            ' ADD CONSTRAINT ' || messages_chat_id_fk_name ||
            ' FOREIGN KEY (chat_id, author) REFERENCES private.personal_chats_members(chat_id, user_id)' ||
            ' ON DELETE CASCADE ON UPDATE CASCADE' ||
            ' ADD CONSTRAINT ' || messages_reply_to_fk_name ||
            ' FOREIGN KEY (reply_to) REFERENCES ' || messages_table_name || '(message_id)' ||
            ' ON DELETE SET NULL ON UPDATE CASCADE';

    EXECUTE 'CREATE TABLE ' || attachments_table_name ||
            ' PARTITION OF private.personal_messages_attachments FOR VALUES IN (''' ||
            initializing_chat_id::text || ''')';

    EXECUTE 'ALTER TABLE ' || attachments_table_name ||
            ' ADD CONSTRAINT ' || attachments_chat_id_message_id_fk_name ||
            ' FOREIGN KEY (chat_id, message_id) REFERENCES ' || messages_table_name || '(chat_id, message_id)' ||
            ' ON DELETE CASCADE ON UPDATE CASCADE' ||
            ' ADD CONSTRAINT ' || attachments_attachment_id_fk_name ||
            ' FOREIGN KEY (attachment_id) REFERENCES private.media(media_id)' ||
            ' ON UPDATE CASCADE';

    RETURN initializing_chat_id;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.send_personal_message(chat_id uuid, author uuid, message_text text, attachments media_file[], reply_to uuid DEFAULT NULL)
RETURNS uuid
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    sending_message_id uuid;
    adding_attachment media_file;
BEGIN
    SELECT private.update_user_online_status(author);

    UPDATE private.personal_chats_members
    SET was_in_chat = CURRENT_TIMESTAMP
    WHERE chat_id = send_personal_message.chat_id
      AND user_id = author;

    IF exists(
        SELECT 1
        FROM private.users_blocks
        WHERE (user_id = author
                   OR block_by = author
            ) AND chat_id = chat_id
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF coalesce(trim(message_text), '') = ''
           AND (
               attachments IS NULL
                   OR array_length(attachments, 1) = 0
               ) THEN
        RAISE DATA_EXCEPTION;
    END IF;

    INSERT INTO private.personal_messages (message_id, chat_id, author, message_text, sent_at, is_updated, updated_at, reply_to) VALUES
        (gen_random_uuid(), send_personal_message.chat_id, send_personal_message.author,
         coalesce(trim(send_personal_message.message_text), ''), CURRENT_TIMESTAMP, false, null, send_personal_message.reply_to)
    RETURNING message_id INTO sending_message_id;

    IF adding_attachment IS NOT NULL THEN
        FOREACH adding_attachment IN ARRAY attachments LOOP
            INSERT INTO private.media (media_id, file_name, content_type)
            VALUES (adding_attachment.media_id, adding_attachment.file_name, adding_attachment.content_type);
            INSERT INTO private.personal_messages_attachments (attachment_id, message_id, chat_id)
            VALUES (adding_attachment.media_id, sending_message_id, send_personal_message.chat_id);
        END LOOP;
    END IF;

    RETURN sending_message_id;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.resend_to_private_messages(chat_id uuid, author uuid, source_chat_type en_chat_type, source_chat_id uuid, messages_id uuid[])
RETURNS SETOF uuid
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    msg record;
    new_message_id uuid;
    message_map jsonb := '{}'::jsonb;
    new_reply_to uuid;
BEGIN
    SELECT private.update_user_online_status(author);

    UPDATE private.personal_chats_members
    SET was_in_chat = current_timestamp
    WHERE chat_id = resend_to_private_messages.chat_id
      AND user_id = author;

    IF exists(
        SELECT 1
        FROM private.users_blocks
        WHERE (user_id = author
                   OR block_by = author
            ) AND chat_id = chat_id
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    FOR msg IN
        SELECT gm.*
        FROM unnest(messages_id) AS mid
        CROSS JOIN LATERAL
            private.get_message(source_chat_id,mid,author,source_chat_type) gm
        ORDER BY gm.sent_at
    LOOP
        new_message_id := gen_random_uuid();

        IF msg.reply_to IS NOT NULL
           AND message_map ? msg.reply_to::text THEN
            new_reply_to :=
                (message_map ->> msg.reply_to::text)::uuid;
        ELSE
            new_reply_to := NULL;
        END IF;

        INSERT INTO private.personal_messages (message_id, chat_id, author, message_text, sent_at, reply_to, resent_from, is_bot_resend)
        VALUES (new_message_id, resend_to_private_messages.chat_id, resend_to_private_messages.author, msg.message_text,
            current_timestamp,new_reply_to,msg.author, (source_chat_type = 'Bot'));

        message_map := message_map ||
            jsonb_build_object(msg.message_id::text, new_message_id::text);

        IF msg.attached_media IS NOT NULL THEN
            INSERT INTO private.personal_messages_attachments (attachment_id, message_id, chat_id)
            SELECT unnest(msg.attached_media), new_message_id, chat_id;
            UPDATE private.media
            SET links_count = links_count + 1
            WHERE media_id IN (msg.attached_media);
        END IF;
        RETURN NEXT new_message_id;
    END LOOP;
    RETURN;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.update_personal_message_text(chat_id uuid, message_id uuid, author uuid, new_message_text text)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    trimmed_message_text text = trim(new_message_text);
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(author);

    IF exists(
        SELECT 1
        FROM private.users_blocks
        WHERE (user_id = author
                   OR block_by = author
            ) AND chat_id = chat_id
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF exists(
        SELECT 1
        FROM private.personal_messages
        WHERE private.personal_messages.chat_id = update_personal_message_text.chat_id
          AND private.personal_messages.message_id = update_personal_message_text.message_id
          AND resent_from IS NOT NULL
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF exists
        (
            SELECT 1
            FROM private.personal_messages_attachments pma
            WHERE pma.chat_id = update_personal_message_text.chat_id
            AND pma.message_id = update_personal_message_text.message_id
        )
        OR
        (
            new_message_text IS NOT NULL
            AND length(trimmed_message_text) > 0
        ) THEN

    UPDATE private.personal_messages
    SET message_text = coalesce(trimmed_message_text, ''),
        is_updated = true,
        updated_at = CURRENT_TIMESTAMP
    WHERE personal_messages.chat_id = update_personal_message_text.chat_id
      AND personal_messages.message_id = update_personal_message_text.message_id
      AND personal_messages.author = update_personal_message_text.author
      AND personal_messages.message_text != coalesce(trimmed_message_text, '');
    ELSE
        RAISE DATA_EXCEPTION;
    END IF;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.block_user(blocking_by uuid, user_id uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    INSERT INTO private.users_blocks (user_id, block_by) VALUES
        (block_user.user_id, blocking_by)
    ON CONFLICT DO NOTHING;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.unblock_user(unblocking_by uuid, user_id uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    DELETE
    FROM private.users_blocks ub
    WHERE ub.user_id = unblock_user.user_id AND ub.block_by = unblocking_by;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.delete_personal_message(chat_id uuid, message_id uuid, deleting_by uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(deleting_by);

    DELETE FROM private.personal_messages
    WHERE personal_messages.chat_id = delete_personal_message.chat_id
      AND personal_messages.message_id = delete_personal_message.message_id
      AND personal_messages.author = deleting_by;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.delete_file_from_personal_message(chat_id uuid, attachment_id uuid, deleting_by uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(deleting_by);

    IF exists(
        SELECT 1
        FROM private.personal_messages
        JOIN private.personal_messages_attachments ON personal_messages.message_id = personal_messages_attachments.message_id
        WHERE personal_messages.chat_id = delete_file_from_personal_message.chat_id
          AND personal_messages_attachments.attachment_id = delete_file_from_personal_message.attachment_id
          AND resent_from IS NOT NULL
        ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    WITH deleted AS (
    DELETE FROM private.personal_messages_attachments
    WHERE personal_messages_attachments.chat_id = delete_file_from_personal_message.chat_id
      AND personal_messages_attachments.attachment_id = delete_file_from_personal_message.attachment_id
      AND exists(
        SELECT 1
        FROM private.personal_messages
        JOIN private.personal_messages_attachments ON personal_messages.message_id = personal_messages_attachments.message_id
        WHERE personal_messages.chat_id = delete_file_from_personal_message.chat_id
          AND personal_messages_attachments.attachment_id = delete_file_from_personal_message.attachment_id
          AND author = deleting_by
        )
    RETURNING attachment_id)

    UPDATE private.media
    SET links_count = links_count - 1
    WHERE media_id IN (
        SELECT deleted.attachment_id
        FROM deleted
        );

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.delete_personal_chat(chat_id uuid, deleting_by uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    attachments_table_name text = 'private.personal_messages_attachments_' || replace(chat_id::text, '-', '_');
    messages_table_name text = 'private.personal_messages_' || replace(chat_id::text, '-', '_');
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(deleting_by);

    IF NOT exists(
        SELECT 1
        FROM private.personal_chats_members pcm
        WHERE pcm.chat_id = delete_personal_chat.chat_id AND user_id = deleting_by
    ) THEN
        RAISE NO_DATA_FOUND;
    END IF;

    WITH deleted AS (
    DELETE
    FROM private.personal_messages_attachments pma
    WHERE pma.chat_id = delete_personal_chat.chat_id
    RETURNING attachment_id)

    UPDATE private.media
    SET links_count = links_count - 1
    WHERE media_id IN (
        SELECT attachment_id
        FROM deleted
        );

    EXECUTE 'DROP TABLE IF EXISTS ' || attachments_table_name;
    EXECUTE 'DROP TABLE IF EXISTS ' || messages_table_name;

    DELETE FROM private.personal_chats
    WHERE personal_chats.chat_id = delete_personal_chat.chat_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;




CREATE OR REPLACE FUNCTION sch_user.search_chats(name_part varchar(64), getting_by uuid, page_number dom_positive_int, page_size dom_positive_int)
RETURNS SETOF public_chat_full_information
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    SELECT pc.chat_name, pc.avatar, array_agg(
        ROW(
            pcm.user_id,
            concat(u.first_name, ' ', u.last_name),
            u.avatar,
            pcm.role
        )::chat_member_info
    ) AS members
    FROM private.public_chats pc
    JOIN private.public_chats_members pcm
        ON pc.chat_id = pcm.chat_id
    JOIN private.users u
        ON pcm.user_id = u.user_id
    WHERE pc.chat_name ILIKE '%' || name_part || '%'
      AND (
            pc.is_searchable
            OR exists (
                SELECT 1
                FROM private.public_chats_members pcm2
                WHERE pcm2.chat_id = pc.chat_id
                  AND pcm2.user_id = getting_by
            )
          )
    GROUP BY pc.chat_id, pc.chat_name, pc.avatar
    ORDER BY similarity(chat_name, name_part) DESC, count(pcm.user_id) DESC
    OFFSET (page_number - 1) * page_size LIMIT page_size;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_public_chat_full_information(chat_id uuid, getting_by uuid)
RETURNS public_chat_full_information
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    SELECT pc.chat_name, pc.avatar, array_agg(ROW(pcm.user_id, concat(users.first_name, ' ', users.last_name), users.avatar, pcm.role)::chat_member_info) AS members
    FROM private.public_chats pc
    JOIN private.public_chats_members pcm ON pc.chat_id = pcm.chat_id
    JOIN private.users ON pcm.user_id = users.user_id
    WHERE pc.chat_id = get_public_chat_full_information.chat_id
      AND exists(
        SELECT 1
        FROM private.public_chats_members
        WHERE public_chats_members.chat_id = get_public_chat_full_information.chat_id
          AND public_chats_members.user_id = getting_by)
    GROUP BY pc.chat_name, pc.avatar;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_public_chat_short_info(chat_id uuid, getting_by uuid)
RETURNS chat_information
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    SELECT pc.chat_id, pc.chat_name, coalesce(nmc.new_messages_count, 0), pc.avatar, 'Public' :: en_chat_type
    FROM private.public_chats pc
    LEFT JOIN (
        SELECT count(pm.message_id) AS new_messages_count, pm.chat_id
        FROM private.public_messages pm
        JOIN private.public_chats_members pcm_self
            ON pm.chat_id = pcm_self.chat_id
        WHERE pm.sent_at > pcm_self.was_in_chat
          AND pcm_self.user_id = getting_by
          AND pm.author != getting_by
          AND pm.chat_id = get_public_chat_short_info.chat_id
        GROUP BY pm.chat_id
    ) nmc ON nmc.chat_id = pc.chat_id
    WHERE pc.chat_id = get_public_chat_short_info.chat_id
      AND EXISTS (
            SELECT 1
            FROM private.public_chats_members pcm2
            WHERE pcm2.chat_id = pc.chat_id
              AND pcm2.user_id = getting_by
      );
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_public_chat_options(chat_id uuid, getting_by uuid)
RETURNS public_chat_options
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    SELECT pc.chat_name, pc.is_searchable, pc.avatar, pc.default_member_role
    FROM private.public_chats pc
    WHERE pc.chat_id = get_public_chat_options.chat_id
      AND exists(
      SELECT 1
      FROM private.public_chats_members pcm
      WHERE pcm.chat_id = get_public_chat_options.chat_id
        AND user_id = getting_by
        AND role IN ('Administrator', 'Creator')
      );
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_banned_users(chat_id uuid, getting_by uuid)
RETURNS SETOF public_chat_banned_user
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    SELECT user_id, banned_by, banned_at
    FROM private.public_chats_banned_users pcbu
    WHERE pcbu.chat_id = get_banned_users.chat_id
      AND exists(
      SELECT 1
      FROM private.public_chats_members pcm
      WHERE pcm.chat_id = get_banned_users.chat_id
        AND user_id = getting_by
        AND role IN ('Administrator', 'Creator')
      );
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_messages_from_public_chat
    (chat_id uuid, getting_by uuid, messages_count dom_positive_int, sent_before timestamp DEFAULT CURRENT_TIMESTAMP)
RETURNS SETOF message
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    UPDATE private.public_chats_members
    SET was_in_chat = CURRENT_TIMESTAMP
    WHERE chat_id = get_messages_from_public_chat.chat_id
      AND user_id = getting_by;

    SELECT pm.message_id, author, message_text, sent_at, is_updated, updated_at, pm.reply_to, pm.resent_from, pm.is_bot_resend,
           coalesce(array_agg(pma.attachment_id) FILTER (WHERE attachment_id IS NOT NULL), '{}')
    FROM private.public_messages pm
    LEFT JOIN private.public_messages_attachments pma ON pm.chat_id = pma.chat_id AND pm.message_id = pma.message_id
    WHERE pm.chat_id = get_messages_from_public_chat.chat_id
      AND exists(
        SELECT 1
        FROM private.public_chats_members pcm
        WHERE pcm.chat_id = get_messages_from_public_chat.chat_id
          AND pcm.user_id = getting_by)
      AND sent_at < coalesce(sent_before, CURRENT_TIMESTAMP)
    GROUP BY pm.message_id, author, message_text, sent_at, is_updated, updated_at, pm.reply_to, pm.resent_from, pm.is_bot_resend, pm.sent_at
    ORDER BY pm.sent_at DESC
    LIMIT messages_count
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_public_message(chat_id uuid, message_id uuid, getting_by uuid)
RETURNS message
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    UPDATE private.public_chats_members
    SET was_in_chat = CURRENT_TIMESTAMP
    WHERE chat_id = get_public_message.chat_id
      AND user_id = getting_by;

    SELECT pm.message_id, author, message_text, sent_at, is_updated, updated_at, pm.reply_to, pm.resent_from, pm.is_bot_resend,
           coalesce(array_agg(pma.attachment_id) FILTER (WHERE attachment_id IS NOT NULL), '{}')
    FROM private.public_messages pm
    LEFT JOIN private.public_messages_attachments pma ON pm.chat_id = pma.chat_id AND pm.message_id = pma.message_id
    WHERE pm.chat_id = get_public_message.chat_id
      AND exists(
        SELECT 1
        FROM private.public_chats_members pcm
        WHERE pcm.chat_id = get_public_message.chat_id
          AND pcm.user_id = getting_by)
      AND pm.message_id = get_public_message.message_id
    GROUP BY pm.message_id, author, message_text, sent_at, is_updated, updated_at, pm.reply_to, pm.resent_from, pm.is_bot_resend
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.audit_chat(chat_id uuid, getting_by uuid, page_number dom_positive_int, page_size dom_positive_int)
RETURNS SETOF audit_log_record
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    SELECT pcal.action_datetime, pcal.source_user_id, pcal.destination_user_id, pcal.action
    FROM public_chats_audit_logs pcal
    WHERE pcal.chat_id = audit_chat.chat_id
      AND exists(
        SELECT 1
        FROM private.public_chats_members pcm
        WHERE pcm.chat_id = audit_chat.chat_id
          AND pcm.user_id = getting_by
          AND pcm.role IN ('Administrator', 'Creator'))
    ORDER BY action_datetime DESC
    OFFSET (page_number - 1) * page_size LIMIT page_size;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.initialize_new_public_chat(chat_name dom_public_chat_name, creator_id uuid, is_searchable bool DEFAULT false, avatar media_file DEFAULT NULL, default_member_role en_public_chat_member_role DEFAULT 'Member')
RETURNS uuid
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    initializing_chat_id uuid = gen_random_uuid();
    chat_id_converted_to_text text = replace(initializing_chat_id::text, '-', '_');
    members_table_name text = 'private.public_chats_members_' || chat_id_converted_to_text;
    members_fk_to_chats_name text = 'public_chats_members_' || chat_id_converted_to_text || '_chat_id_fkey';
    members_fk_to_users_name text = 'public_chats_members_' || chat_id_converted_to_text || '_user_id_fkey';
    banned_table_name text = 'private.public_chats_banned_users_' || chat_id_converted_to_text;
    banned_fk_to_chats_members_name text = 'public_chats_banned_users_' || chat_id_converted_to_text || '_chat_id_member_id_fkey';
    banned_fk_to_users_name text = 'public_chats_banned_users_' || chat_id_converted_to_text || '_user_id_fkey';
    messages_table_name text = 'private.public_messages_' || chat_id_converted_to_text;
    messages_fk_to_chats_name text = 'public_messages_' || chat_id_converted_to_text || '_chat_id_fkey';
    messages_fk_to_users_name text = 'public_messages_' || chat_id_converted_to_text || '_user_id_fkey';
    messages_reply_to_fk_name text = 'public_messages_' || chat_id_converted_to_text || '_reply_to_fkey';
    attachments_table_name text = 'private.public_messages_attachments_' || chat_id_converted_to_text;
    attachments_fk_to_messages_name text = 'public_messages_attachments_' || chat_id_converted_to_text || '_chat_id_message_id_fkey';
    attachments_fk_to_media_name text = 'public_messages_attachments_' || chat_id_converted_to_text || '_media_id_fkey';
    audit_table_name text = 'private.public_chats_audit_logs_' || chat_id_converted_to_text;
    audit_fk_to_chats_name text = 'public_chats_audit_logs_' || chat_id_converted_to_text || '_chat_id_fkey';
    audit_fk_to_src_user_id_name text = 'public_chats_audit_logs_' || chat_id_converted_to_text || 'source_user_id_fkey';
    audit_fk_to_dest_user_id_name text = 'public_chats_audit_logs_' || chat_id_converted_to_text || 'destination_user_id_fkey';
BEGIN
    IF (avatar IS NOT NULL) THEN
        INSERT INTO private.media (media_id, file_name, content_type)
        VALUES (avatar.media_id, avatar.file_name, avatar.content_type);
    END IF;

    INSERT INTO private.public_chats (chat_id, chat_name, avatar, is_searchable, default_member_role)
    VALUES (initializing_chat_id, initialize_new_public_chat.chat_name, initialize_new_public_chat.avatar.media_id, initialize_new_public_chat.is_searchable, initialize_new_public_chat.default_member_role);

    EXECUTE 'CREATE TABLE ' || members_table_name ||
            ' PARTITION OF private.public_chats_members FOR VALUES IN (''' ||
            initializing_chat_id::text || ''')';

    EXECUTE 'ALTER TABLE ' || members_table_name ||
            ' ADD CONSTRAINT ' || members_fk_to_chats_name ||
            ' FOREIGN KEY (chat_id) REFERENCES private.public_chats(chat_id) ON DELETE CASCADE ON UPDATE CASCADE,' ||
            ' ADD CONSTRAINT ' || members_fk_to_users_name ||
            ' FOREIGN KEY (user_id) REFERENCES private.users(user_id) ON DELETE SET NULL ON UPDATE CASCADE';

    EXECUTE 'CREATE TABLE ' || banned_table_name ||
            ' PARTITION OF private.public_chats_banned_users FOR VALUES IN (''' ||
            initializing_chat_id::text || ''')';

    EXECUTE 'ALTER TABLE ' || banned_table_name ||
            ' ADD CONSTRAINT ' || banned_fk_to_chats_members_name ||
            ' FOREIGN KEY (chat_id, banned_by) REFERENCES private.public_chats_members(chat_id, user_id) ON DELETE SET NULL ON UPDATE CASCADE,' ||
            ' ADD CONSTRAINT ' || banned_fk_to_users_name ||
            ' FOREIGN KEY (user_id) REFERENCES private.users(user_id) ON DELETE CASCADE ON UPDATE CASCADE';

    EXECUTE 'CREATE TABLE ' || messages_table_name ||
            ' PARTITION OF private.public_messages FOR VALUES IN (''' ||
            initializing_chat_id::text || ''')';

    EXECUTE 'ALTER TABLE ' || messages_table_name ||
            ' ADD CONSTRAINT ' || messages_fk_to_chats_name ||
            ' FOREIGN KEY (chat_id) REFERENCES private.public_chats(chat_id)' ||
            ' ON DELETE CASCADE ON UPDATE CASCADE' ||
            ' ADD CONSTRAINT ' || messages_fk_to_users_name ||
            ' FOREIGN KEY (author) REFERENCES private.users(user_id)' ||
            ' ON DELETE SET NULL ON UPDATE CASCADE' ||
            ' ADD CONSTRAINT ' || messages_reply_to_fk_name ||
            ' FOREIGN KEY (reply_to) REFERENCES ' || messages_table_name || '(message_id)' ||
            ' ON DELETE SET NULL ON UPDATE CASCADE';

    EXECUTE 'CREATE TABLE ' || attachments_table_name ||
            ' PARTITION OF private.public_messages_attachments FOR VALUES IN (''' ||
            initializing_chat_id::text || ''')';

    EXECUTE 'ALTER TABLE ' || attachments_table_name ||
            ' ADD CONSTRAINT ' || attachments_fk_to_messages_name ||
            ' FOREIGN KEY (chat_id, message_id) REFERENCES ' || messages_table_name || '(chat_id, message_id)' ||
            ' ON DELETE CASCADE ON UPDATE CASCADE' ||
            ' ADD CONSTRAINT ' || attachments_fk_to_media_name ||
            ' FOREIGN KEY (attachment_id) REFERENCES private.media(media_id)' ||
            ' ON UPDATE CASCADE';

    EXECUTE 'CREATE TABLE ' || audit_table_name ||
            ' PARTITION OF private.public_chats_audit_logs FOR VALUES IN (''' ||
            initializing_chat_id::text || ''')';

    EXECUTE 'ALTER TABLE ' || audit_table_name ||
            ' ADD CONSTRAINT ' || audit_fk_to_chats_name ||
            ' FOREIGN KEY (chat_id) REFERENCES private.public_chats(chat_id)' ||
            ' ON DELETE CASCADE ON UPDATE CASCADE' ||
            ' ADD CONSTRAINT ' || audit_fk_to_src_user_id_name ||
            ' FOREIGN KEY (source_user_id) REFERENCES private.users(user_id)' ||
            ' ON DELETE SET NULL ON UPDATE CASCADE' ||
            ' ADD CONSTRAINT ' || audit_fk_to_dest_user_id_name ||
            ' FOREIGN KEY (destination_user_id) REFERENCES private.users(user_id)' ||
            ' ON DELETE SET NULL ON UPDATE CASCADE';

    INSERT INTO private.public_chats_members (chat_id, user_id, role) VALUES
        (initializing_chat_id, creator_id, 'Creator');

    SELECT private.update_user_online_status(creator_id);

    RETURN initializing_chat_id;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.join_chat(chat_id uuid, user_id uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(user_id);

    IF exists(
        SELECT 1
        FROM public_chats_banned_users pcbu
        WHERE pcbu.chat_id = join_chat.chat_id
          AND pcbu.user_id = join_chat.user_id
    ) THEN
        RAISE EXCEPTION 'User is banned in chat'
            USING ERRCODE = '42501';
    END IF;

    INSERT INTO public_chats_members (chat_id, user_id, role)
    VALUES (join_chat.chat_id, join_chat.user_id, (
        SELECT default_member_role
        FROM public_chats pc
        WHERE pc.chat_id = join_chat.chat_id
        LIMIT 1
        ));

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows > 0 THEN
        INSERT INTO public_chats_audit_logs (chat_id, source_user_id, destination_user_id, action)
        VALUES (chat_id, user_id, user_id, 'Join');
    END IF;

    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.send_public_message(chat_id uuid, author uuid, message_text text, attachments media_file[], reply_to uuid DEFAULT NULL)
RETURNS uuid
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    sending_message_id uuid;
    adding_attachment media_file;
BEGIN
    SELECT private.update_user_online_status(author);

    IF NOT exists(
        SELECT 1
        FROM public_chats_members pcm
        WHERE pcm.chat_id = send_public_message.chat_id
          AND pcm.user_id = author
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    UPDATE private.public_chats_members
    SET was_in_chat = CURRENT_TIMESTAMP
    WHERE chat_id = send_public_message.chat_id
      AND user_id = author;

    IF exists(
        SELECT 1
        FROM private.public_chats_banned_users pcbu
        WHERE pcbu.user_id = author
          AND pcbu.chat_id = send_public_message.chat_id
    ) THEN
        RAISE EXCEPTION 'User is banned in chat'
            USING ERRCODE = '42501';
    END IF;

    IF exists(
        SELECT 1
        FROM public_chats_members pcm
        WHERE pcm.chat_id = send_public_message.chat_id
          AND pcm.user_id = author
          AND pcm.role = 'Reader'
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF coalesce(trim(message_text), '') = ''
           AND (
               attachments IS NULL
                   OR array_length(attachments, 1) = 0
               ) THEN
        RAISE DATA_EXCEPTION;
    END IF;

    INSERT INTO private.public_messages (message_id, chat_id, author, message_text, sent_at, is_updated, updated_at, reply_to) VALUES
        (gen_random_uuid(), send_public_message.chat_id, send_public_message.author,
         coalesce(trim(send_public_message.message_text), ''), CURRENT_TIMESTAMP, false, null, send_public_message.reply_to)
    RETURNING message_id INTO sending_message_id;

    IF adding_attachment IS NOT NULL THEN
        FOREACH adding_attachment IN ARRAY attachments LOOP
            INSERT INTO private.media (media_id, file_name, content_type)
            VALUES (adding_attachment.media_id, adding_attachment.file_name, adding_attachment.content_type);
            INSERT INTO private.public_messages_attachments (attachment_id, message_id, chat_id)
            VALUES (adding_attachment.media_id, sending_message_id, send_public_message.chat_id);
        END LOOP;
    END IF;

    RETURN sending_message_id;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.resend_to_public_messages(chat_id uuid, author uuid, source_chat_type en_chat_type, source_chat_id uuid, messages_id uuid[])
RETURNS SETOF uuid
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    msg message;
    new_message_id uuid;
    message_map jsonb := '{}'::jsonb;
    new_reply_to uuid;
BEGIN
    SELECT private.update_user_online_status(author);

    IF NOT exists(
        SELECT 1
        FROM public_chats_members pcm
        WHERE pcm.chat_id = resend_to_public_messages.chat_id
          AND pcm.user_id = author
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    UPDATE private.public_chats_members
    SET was_in_chat = current_timestamp
    WHERE chat_id = resend_to_public_messages.chat_id
      AND user_id = author;

    IF exists(
        SELECT 1
        FROM private.public_chats_banned_users pcbu
        WHERE pcbu.user_id = author
          AND pcbu.chat_id = resend_to_public_messages.chat_id
    ) THEN
        RAISE EXCEPTION 'User is banned in chat'
            USING ERRCODE = '42501';
    END IF;

    IF exists(
        SELECT 1
        FROM public_chats_members pcm
        WHERE pcm.chat_id = resend_to_public_messages.chat_id
          AND pcm.user_id = author
          AND pcm.role = 'Reader'
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    FOR msg IN
        SELECT gm.*
        FROM unnest(messages_id) AS mid
        CROSS JOIN LATERAL
            private.get_message(source_chat_id,mid,author,source_chat_type) gm
        ORDER BY gm.sent_at
    LOOP
        new_message_id := gen_random_uuid();

        IF msg.reply_to IS NOT NULL
           AND message_map ? msg.reply_to::text THEN
            new_reply_to :=
                (message_map ->> msg.reply_to::text)::uuid;
        ELSE
            new_reply_to := NULL;
        END IF;

        INSERT INTO private.public_messages (message_id, chat_id, author, message_text, sent_at, reply_to, resent_from, is_bot_resend)
        VALUES (new_message_id, resend_to_public_messages.chat_id, author, msg.message_text,
                current_timestamp,new_reply_to, msg.author, (source_chat_type = 'Bot'));

        message_map := message_map ||
            jsonb_build_object(msg.message_id::text, new_message_id::text);

        IF msg.attached_media IS NOT NULL THEN
            INSERT INTO private.public_messages_attachments (attachment_id, message_id, chat_id)
            SELECT unnest(msg.attached_media),new_message_id, resend_to_public_messages.chat_id;

            UPDATE private.media
            SET links_count = links_count + 1
            WHERE media_id IN (msg.attached_media);
        END IF;

        RETURN NEXT new_message_id;
    END LOOP;
    RETURN;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.update_public_text_message(chat_id uuid, message_id uuid, author uuid, new_message_text text)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    trimmed_message_text text = trim(new_message_text);
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(author);

    IF exists(
        SELECT 1
        FROM private.public_chats_banned_users pcbu
        WHERE pcbu.chat_id = update_public_text_message.chat_id
          AND pcbu.user_id = author
    ) THEN
        RAISE EXCEPTION 'User is banned in chat'
            USING ERRCODE = '42501';
    END IF;

    IF NOT exists(
        SELECT 1
        FROM public_chats_members pcm
        WHERE pcm.chat_id = update_public_text_message.chat_id
          AND pcm.user_id = author
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF exists(
        SELECT 1
        FROM public_chats_members pcm
        WHERE pcm.chat_id = update_public_text_message.chat_id
          AND pcm.user_id = author
          AND pcm.role = 'Reader'
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF exists(
        SELECT 1
        FROM public_messages
        WHERE public_messages.chat_id = update_public_text_message.chat_id
          AND public_messages.message_id = update_public_text_message.message_id
          AND resent_from IS NOT NULL
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF exists
        (
            SELECT 1
            FROM private.public_messages_attachments
            WHERE public_messages_attachments.chat_id = update_public_text_message.chat_id
              AND public_messages_attachments.message_id = update_public_text_message.message_id
        )
        OR
        (
            new_message_text IS NOT NULL
            AND length(trimmed_message_text) > 0
        ) THEN

    UPDATE private.public_messages
    SET message_text = coalesce(trimmed_message_text, ''),
        is_updated = true,
        updated_at = CURRENT_TIMESTAMP
    WHERE public_messages.chat_id = update_public_text_message.chat_id
      AND public_messages.message_id = update_public_text_message.message_id
      AND public_messages.author = update_public_text_message.author
      AND public_messages.message_text != coalesce(trimmed_message_text, '');
    ELSE
        RAISE DATA_EXCEPTION;
    END IF;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows > 0 THEN
        INSERT INTO public_chats_audit_logs (chat_id, source_user_id, destination_user_id, action)
        VALUES (chat_id, author, author, 'UpdateMessage');
    END IF;

    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.delete_public_message(chat_id uuid, message_id uuid, deleting_by uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
    deleting_message private.public_messages;
BEGIN
    IF exists(
        SELECT 1
        FROM private.public_chats_banned_users pcbu
        WHERE pcbu.chat_id = delete_public_message.chat_id
          AND pcbu.user_id = deleting_by
    ) THEN
        RAISE EXCEPTION 'User is banned in chat'
            USING ERRCODE = '42501';
    END IF;

    SELECT * INTO deleting_message
    FROM private.public_messages
    WHERE public_messages.chat_id = delete_public_message.chat_id
      AND public_messages.message_id = delete_public_message.message_id;

    IF deleting_message.author != deleting_by AND
       (SELECT coalesce(role, '')
        FROM private.public_chats_members
        WHERE public_chats_members.chat_id = delete_public_message.chat_id
          AND user_id = delete_public_message.deleting_by) NOT IN ('Creator', 'Administrator') THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF exists(
        SELECT 1
        FROM public_chats_members pcm
        WHERE user_id = deleting_by
        AND pcm.chat_id = delete_public_message.chat_id
        AND role = 'Reader'
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    SELECT private.update_user_online_status(deleting_by);

    DELETE FROM private.public_messages
    WHERE public_messages.chat_id = delete_public_message.chat_id
      AND public_messages.message_id = delete_public_message.message_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows > 0 THEN
        INSERT INTO public_chats_audit_logs (chat_id, source_user_id, destination_user_id, action)
        VALUES (chat_id, deleting_by, deleting_message.author, 'DeleteMessage');
    END IF;

    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.delete_file_from_public_message(chat_id uuid, attachment_id uuid, deleting_by uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
    message_with_attachment private.public_messages;
BEGIN
    IF exists(
        SELECT 1
        FROM private.public_chats_banned_users pcbu
        WHERE pcbu.chat_id = delete_file_from_public_message.chat_id
          AND pcbu.user_id = deleting_by
    ) THEN
        RAISE EXCEPTION 'User is banned in chat'
            USING ERRCODE = '42501';
    END IF;

    SELECT public_messages.* INTO message_with_attachment
    FROM private.public_messages
    JOIN private.public_messages_attachments
        ON public_messages.chat_id = public_messages_attachments.chat_id
       AND public_messages.message_id = public_messages_attachments.message_id
    WHERE public_messages.chat_id = delete_file_from_public_message.chat_id
      AND public_messages_attachments.attachment_id = delete_file_from_public_message.attachment_id;

    IF message_with_attachment.author != deleting_by AND
       (SELECT coalesce(role :: text, 'Not a member')
        FROM private.public_chats_members
        WHERE public_chats_members.chat_id = delete_file_from_public_message.chat_id
          AND user_id = deleting_by) NOT IN ('Creator', 'Administrator') THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF exists(
        SELECT 1
        FROM public_chats_members pcm
        WHERE user_id = deleting_by
        AND pcm.chat_id = delete_file_from_public_message.chat_id
        AND role = 'Reader'
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    SELECT private.update_user_online_status(deleting_by);

    IF exists(
        SELECT 1
        FROM private.public_messages
        JOIN private.public_messages_attachments ON public_messages.message_id = public_messages_attachments.message_id
        WHERE public_messages.chat_id = delete_file_from_public_message.chat_id
          AND public_messages_attachments.attachment_id = delete_file_from_public_message.attachment_id
          AND resent_from IS NOT NULL
        ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    WITH deleted AS (
    DELETE FROM private.public_messages_attachments
    WHERE public_messages_attachments.chat_id = delete_file_from_public_message.chat_id
      AND public_messages_attachments.attachment_id = delete_file_from_public_message.attachment_id
    RETURNING attachment_id)

    UPDATE private.media
    SET links_count = links_count - 1
    WHERE media_id IN (
        SELECT deleted.attachment_id
        FROM deleted
        );

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows > 0 THEN
        INSERT INTO public_chats_audit_logs (chat_id, source_user_id, destination_user_id, action)
        VALUES (chat_id, deleting_by, message_with_attachment.author, 'DeleteAttachment');
    END IF;

    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.delete_public_chat(chat_id uuid, deleting_by uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    chat_id_converted_to_text text = replace(chat_id::text, '-', '_');
    attachments_table_name text = 'private.public_messages_attachments_' || chat_id_converted_to_text;
    messages_table_name text = 'private.public_messages_' || chat_id_converted_to_text;
    members_table_name text = 'private.public_chats_members_' || chat_id_converted_to_text;
    audit_table_name text = 'private.public_chats_audit_logs_' || chat_id_converted_to_text;
    banned_table_name text = 'private.public_chats_banned_users_' || chat_id_converted_to_text;
    affected_rows int;
BEGIN
    IF (SELECT coalesce(role :: text, 'Not a member')
        FROM private.public_chats_members
        WHERE public_chats_members.chat_id = delete_public_chat.chat_id
          AND user_id = deleting_by) != 'Creator' THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    SELECT private.update_user_online_status(deleting_by);

    WITH deleted AS (
    DELETE
    FROM public_messages_attachments pma
    WHERE pma.chat_id = delete_public_chat.chat_id
    RETURNING attachment_id)

    UPDATE private.media
    SET links_count = links_count - 1
    WHERE media_id IN (
        SELECT attachment_id
        FROM deleted
        );

    EXECUTE 'DROP TABLE IF EXISTS ' || attachments_table_name;
    EXECUTE 'DROP TABLE IF EXISTS ' || messages_table_name;
    EXECUTE 'DROP TABLE IF EXISTS ' || members_table_name;
    EXECUTE 'DROP TABLE IF EXISTS ' || audit_table_name;
    EXECUTE 'DROP TABLE IF EXISTS ' || banned_table_name;

    DELETE FROM media
    WHERE media_id = (
        SELECT avatar
        FROM public_chats pc
        WHERE pc.chat_id = delete_public_chat.chat_id
        LIMIT 1);

    DELETE FROM private.public_chats
    WHERE public_chats.chat_id = delete_public_chat.chat_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.give_public_chat_member_role(chat_id uuid, member_id uuid, role en_public_chat_member_role, giving_by uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
    giving_by_role en_public_chat_member_role;
BEGIN
    IF exists(
        SELECT 1
        FROM private.public_chats_banned_users pcbu
        WHERE pcbu.chat_id = give_public_chat_member_role.chat_id
          AND pcbu.user_id = giving_by
    ) THEN
        RAISE EXCEPTION 'User is banned in chat'
            USING ERRCODE = '42501';
    END IF;

    IF NOT exists(SELECT 1
                  FROM private.public_chats_members
                  WHERE public_chats_members.chat_id = give_public_chat_member_role.chat_id
                    AND user_id = giving_by
                    AND public_chats_members.role IN ('Creator', 'Administrator')) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    SELECT private.update_user_online_status(giving_by);

    IF giving_by_role IS NULL OR NOT exists(
        SELECT 1
        FROM public_chats_members
        WHERE member_id = user_id
    ) THEN
        RAISE NO_DATA_FOUND;
    END IF;

    SELECT pcm.role INTO giving_by_role
    FROM public_chats_members pcm
    WHERE pcm.user_id = giving_by
      AND pcm.chat_id = give_public_chat_member_role.chat_id;

    IF role = 'Creator' AND giving_by_role != 'Creator' THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF role = 'Creator' THEN
        UPDATE private.public_chats_members
        SET role = 'Administrator'
        WHERE public_chats_members.chat_id = give_public_chat_member_role.chat_id
          AND user_id = giving_by;
    END IF;

    UPDATE private.public_chats_members
    SET role = give_public_chat_member_role.role
    WHERE public_chats_members.chat_id = give_public_chat_member_role.chat_id
      AND user_id = give_public_chat_member_role.member_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows > 0 THEN
        INSERT INTO public_chats_audit_logs (chat_id, source_user_id, destination_user_id, action)
        VALUES (chat_id, giving_by, member_id, 'ChangeRole');
    END IF;

    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.delete_member_from_public_chat(chat_id uuid, deleting_by uuid, deleting_user uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
    deleting_by_role en_public_chat_member_role;
    deleting_user_role en_public_chat_member_role;
BEGIN
    IF exists(
        SELECT 1
        FROM private.public_chats_banned_users pcbu
        WHERE pcbu.chat_id = delete_member_from_public_chat.chat_id
          AND pcbu.user_id = deleting_by
    ) THEN
        RAISE EXCEPTION 'User is banned in chat'
            USING ERRCODE = '42501';
    END IF;

    SELECT role INTO deleting_by_role
    FROM private.public_chats_members
    WHERE public_chats_members.chat_id = delete_member_from_public_chat.chat_id
      AND user_id = deleting_by;

    SELECT role INTO deleting_user_role
    FROM private.public_chats_members
    WHERE public_chats_members.chat_id = delete_member_from_public_chat.chat_id
      AND user_id = deleting_user;

    IF deleting_by_role IS NULL OR deleting_user_role IS NULL THEN
        RAISE NO_DATA_FOUND;
    END IF;

    IF NOT deleting_by_role IN ('Creator', 'Administrator') THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF deleting_by = deleting_user THEN
        RAISE DATA_EXCEPTION;
    END IF;

    IF deleting_by_role = 'Administrator' AND deleting_user_role IN ('Administrator', 'Creator') THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    SELECT private.update_user_online_status(deleting_by);

    DELETE FROM private.public_chats_members
    WHERE public_chats_members.chat_id = delete_member_from_public_chat.chat_id
      AND user_id = deleting_user;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows > 0 THEN
        INSERT INTO public_chats_audit_logs (chat_id, source_user_id, destination_user_id, action)
        VALUES (chat_id, deleting_by, deleting_user, 'Kick');
    END IF;

    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.update_public_chat(chat_id uuid, updating_by uuid, update_avatar bool, chat_name dom_public_chat_name DEFAULT NULL, is_searchable bool DEFAULT NULL, avatar media_file DEFAULT NULL, default_member_role en_public_chat_member_role DEFAULT NULL)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    IF exists(
        SELECT 1
        FROM private.public_chats_banned_users pcbu
        WHERE pcbu.chat_id = update_public_chat.chat_id
          AND pcbu.user_id = updating_by
    ) THEN
        RAISE EXCEPTION 'User is banned in chat'
            USING ERRCODE = '42501';
    END IF;

    IF (SELECT coalesce(role :: text, 'Not a member')
        FROM private.public_chats_members
        WHERE public_chats_members.chat_id = update_public_chat.chat_id
          AND user_id = updating_by) NOT IN ('Creator', 'Administrator') THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    SELECT private.update_user_online_status(updating_by);

    IF update_avatar THEN
        DELETE FROM private.media
        WHERE media_id =
            (
                SELECT pc.avatar
                FROM private.public_chats pc
                WHERE pc.chat_id = update_public_chat.chat_id
            );

        IF avatar IS NOT NULL THEN
            INSERT INTO private.media (media_id, file_name, content_type)
            VALUES (avatar.media_id, avatar.file_name, avatar.content_type);

            UPDATE private.public_chats pc
            SET avatar = update_public_chat.avatar.media_id
            WHERE chat_id = update_public_chat.chat_id;
        END IF;
    END IF;

    UPDATE private.public_chats pc
    SET chat_name = coalesce(update_public_chat.chat_name, pc.chat_name),
        avatar = coalesce(update_public_chat.avatar.media_id, pc.avatar),
        is_searchable = coalesce(update_public_chat.is_searchable, pc.is_searchable),
        default_member_role = coalesce(update_public_chat.default_member_role, pc.default_member_role)
    WHERE chat_id = update_public_chat.chat_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows > 0 THEN
        INSERT INTO public_chats_audit_logs (chat_id, source_user_id, destination_user_id, action)
        VALUES (chat_id, updating_by, updating_by, 'UpdateSettings');
    END IF;

    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.ban_user(chat_id uuid, user_id uuid, banning_by uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
    user_role en_public_chat_member_role;
    banning_by_role en_public_chat_member_role;
BEGIN
    IF exists(
        SELECT 1
        FROM private.public_chats_banned_users pcbu
        WHERE pcbu.chat_id = ban_user.chat_id
          AND pcbu.user_id = banning_by
    ) THEN
        RAISE EXCEPTION 'User is banned in chat'
            USING ERRCODE = '42501';
    END IF;

    SELECT role INTO banning_by_role
    FROM private.public_chats_members pcm
    WHERE pcm.chat_id = ban_user.chat_id
      AND pcm.user_id = banning_by;

    SELECT role INTO user_role
    FROM private.public_chats_members pcm
    WHERE pcm.chat_id = ban_user.chat_id
      AND pcm.user_id = ban_user.user_id;

    IF banning_by_role IS NULL OR user_role IS NULL THEN
        RAISE NO_DATA_FOUND;
    END IF;

    IF banning_by_role NOT IN ('Creator', 'Administrator') THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF user_id = banning_by THEN
        RAISE DATA_EXCEPTION;
    END IF;

    IF banning_by_role = 'Administrator'
           AND user_role IN ('Creator', 'Administrator') THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    INSERT INTO private.public_chats_banned_users (chat_id, user_id, banned_by)
    VALUES (ban_user.chat_id, ban_user.user_id, banning_by);

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows > 0 THEN
        INSERT INTO public_chats_audit_logs (chat_id, source_user_id, destination_user_id, action)
        VALUES (chat_id, banning_by, user_id, 'Ban');
    END IF;

    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.unban_user(chat_id uuid, user_id uuid, unbanning_by uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
    user_role en_public_chat_member_role;
    unbanning_by_role en_public_chat_member_role;
BEGIN
    IF exists(
        SELECT 1
        FROM private.public_chats_banned_users pcbu
        WHERE pcbu.chat_id = unban_user.chat_id
          AND pcbu.user_id = unbanning_by
    ) THEN
        RAISE EXCEPTION 'User is banned in chat'
            USING ERRCODE = '42501';
    END IF;

    SELECT role INTO unbanning_by_role
    FROM private.public_chats_members pcm
    WHERE pcm.chat_id = unban_user.chat_id
      AND pcm.user_id = unbanning_by;

    SELECT role INTO user_role
    FROM private.public_chats_members pcm
    WHERE pcm.chat_id = unban_user.chat_id
      AND pcm.user_id = unban_user.user_id;

    IF unbanning_by_role IS NULL OR user_role IS NULL THEN
        RAISE NO_DATA_FOUND;
    END IF;

    IF unbanning_by_role NOT IN ('Creator', 'Administrator') THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF unbanning_by_role = 'Administrator'
           AND user_role IN ('Creator', 'Administrator') THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    DELETE
    FROM private.public_chats_banned_users pcbu
    WHERE pcbu.user_id = unban_user.user_id
      AND pcbu.chat_id = unban_user.chat_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows > 0 THEN
        INSERT INTO public_chats_audit_logs (chat_id, source_user_id, destination_user_id, action)
        VALUES (chat_id, unbanning_by, user_id, 'Unban');
    END IF;

    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.leave_chat(chat_id uuid, user_id uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    IF exists(
        SELECT 1
        FROM private.public_chats_banned_users pcbu
        WHERE pcbu.chat_id = leave_chat.chat_id
          AND pcbu.user_id = leave_chat.user_id
    ) THEN
        RAISE EXCEPTION 'User is banned in chat'
            USING ERRCODE = '42501';
    END IF;

    IF exists(
        SELECT 1
        FROM private.public_chats_members pcm
        WHERE pcm.chat_id = leave_chat.chat_id
          AND pcm.user_id = leave_chat.user_id
          AND role = 'Creator'
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    DELETE
    FROM public_chats_members pcm
    WHERE pcm.chat_id = leave_chat.chat_id
      AND pcm.user_id = leave_chat.user_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;

    IF affected_rows > 0 THEN
        INSERT INTO public_chats_audit_logs (chat_id, source_user_id, destination_user_id, action)
        VALUES (chat_id, user_id, user_id, 'Leave');
    END IF;

    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.delete_and_ban_chat_member(chat_id uuid, user_id uuid, deleting_by uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT sch_user.ban_user(chat_id, user_id, deleting_by);
    SELECT sch_user.delete_member_from_public_chat(chat_id, deleting_by, user_id);
$$
LANGUAGE sql;




CREATE OR REPLACE FUNCTION sch_user.get_bot_chat_short_info(chat_id uuid, getting_by uuid)
RETURNS chat_information
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    SELECT bc.chat_id, coalesce(b.name, 'Deleted bot'), coalesce(nmc.new_messages_count, 0), b.avatar, 'Bot'::en_chat_type
    FROM private.bot_chats bc
    LEFT JOIN private.bots b ON b.bot_id = bc.bot_id
    LEFT JOIN (
        SELECT count(bm.message_id) AS new_messages_count
        FROM private.bot_messages bm
        WHERE bm.chat_id = get_bot_chat_short_info.chat_id
          AND bm.sent_at > (
                SELECT bc2.was_in_chat
                FROM private.bot_chats bc2
                WHERE bc2.chat_id = get_bot_chat_short_info.chat_id
                  AND bc2.user_id = getting_by)
    ) nmc ON true
    WHERE bc.chat_id = get_bot_chat_short_info.chat_id
      AND bc.user_id = getting_by;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_active_buttons_list(chat_id uuid, getting_by uuid)
RETURNS SETOF bot_button_info
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    SELECT button_text, inner_command, background_color
    FROM bot_chats_active_buttons bcab
    WHERE bcab.chat_id = get_active_buttons_list.chat_id AND exists(
        SELECT 1
        FROM bot_chats bc
        WHERE bc.chat_id = get_active_buttons_list.chat_id
          AND user_id = getting_by)
    ORDER BY button_id;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_bot_messages
    (chat_id uuid, getting_by uuid, messages_count dom_positive_int, sent_before timestamp DEFAULT CURRENT_TIMESTAMP)
RETURNS SETOF message
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    UPDATE private.bot_chats
    SET was_in_chat = CURRENT_TIMESTAMP
    WHERE chat_id = get_bot_messages.chat_id
      AND user_id = getting_by;

    SELECT bm.message_id, CASE WHEN bm.is_bot THEN bc.bot_id ELSE bc.user_id END AS author,
        bm.message_text, bm.sent_at, bm.is_updated, bm.updated_at, bm.reply_to, bm.resent_from, bm.is_bot_resend,
        coalesce(att.attached_media, '{}')
    FROM private.bot_messages bm
    JOIN private.bot_chats bc ON bc.chat_id = bm.chat_id
    LEFT JOIN (
        SELECT bma.chat_id, bma.message_id, array_agg(bma.attachment_id) AS attached_media
        FROM private.bot_messages_attachments bma
        GROUP BY bma.chat_id, bma.message_id
    ) att ON att.chat_id = bm.chat_id AND att.message_id = bm.message_id
    WHERE bm.chat_id = get_bot_messages.chat_id AND bc.user_id = get_bot_messages.getting_by AND bm.sent_at < get_bot_messages.sent_before
    ORDER BY bm.sent_at DESC
    LIMIT get_bot_messages.messages_count;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_bot_message(chat_id uuid, message_id uuid, getting_by uuid)
RETURNS message
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    UPDATE private.bot_chats
    SET was_in_chat = CURRENT_TIMESTAMP
    WHERE chat_id = get_bot_message.chat_id
      AND user_id = getting_by;

    SELECT bm.message_id, CASE WHEN bm.is_bot THEN bc.bot_id ELSE bc.user_id END AS author,
        bm.message_text, bm.sent_at, bm.is_updated, bm.updated_at, bm.reply_to, bm.resent_from, bm.is_bot_resend,
        coalesce(att.attached_media, '{}')
    FROM private.bot_messages bm
    JOIN private.bot_chats bc ON bc.chat_id = bm.chat_id
    LEFT JOIN LATERAL (
        SELECT array_agg(bma.attachment_id) AS attached_media
        FROM private.bot_messages_attachments bma
        WHERE bma.chat_id = bm.chat_id AND bma.message_id = bm.message_id
    ) att ON true
    WHERE bm.chat_id = get_bot_message.chat_id AND bc.user_id = get_bot_message.getting_by AND bm.message_id = get_bot_message.message_id;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_bot_id_by_chat_id(chat_id uuid, getting_by uuid)
RETURNS uuid
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    SELECT bot_id
    FROM bot_chats bc
    WHERE bc.chat_id = get_bot_id_by_chat_id.chat_id
      AND user_id = getting_by;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.get_bot_chat_ability(chat_id uuid, getting_by uuid)
RETURNS bool
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
    SELECT private.update_user_online_status(getting_by);

    SELECT is_enabled
    FROM bot_chats bc
    WHERE bc.chat_id = get_bot_chat_ability.chat_id
      AND user_id = getting_by;
$$
LANGUAGE sql;

CREATE OR REPLACE FUNCTION sch_user.initialize_new_bot_chat(user_id uuid, bot_id uuid)
RETURNS uuid
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    existing_chat_id uuid;
    initializing_chat_id uuid = gen_random_uuid();
    chat_id_converted_to_text text = replace(initializing_chat_id::text, '-', '_');
    messages_table_name text = 'private.bot_messages_' || chat_id_converted_to_text;
    messages_chat_id_fk_name text = 'bot_messages_' || chat_id_converted_to_text || '_chat_id_fkey';
    messages_reply_to_fk_name text = 'bot_messages_' || chat_id_converted_to_text || '_reply_to_fkey';
    attachments_table_name text = 'private.bot_messages_attachments_' || chat_id_converted_to_text;
    attachments_chat_id_message_id_fk_name text = 'bot_messages_attachments_' || chat_id_converted_to_text || '_chat_id_message_id_fkey';
    attachments_attachment_id_fk_name text = 'bot_messages_attachments_' || chat_id_converted_to_text || '_attachment_id_fkey';
BEGIN
    SELECT private.update_user_online_status(user_id);

    SELECT chat_id INTO existing_chat_id
    FROM bot_chats bc
    WHERE bc.user_id = initialize_new_bot_chat.user_id
      AND bc.bot_id = initialize_new_bot_chat.bot_id;

    IF existing_chat_id IS NOT NULL THEN
        RETURN existing_chat_id;
    END IF;

    INSERT INTO private.bot_chats (chat_id, bot_id, user_id, is_enabled)
    VALUES (initializing_chat_id, initialize_new_bot_chat.bot_id, initialize_new_bot_chat.user_id, true);

    EXECUTE 'CREATE TABLE ' || messages_table_name ||
            ' PARTITION OF private.bot_messages FOR VALUES IN (''' ||
            initializing_chat_id::text || ''')';

    EXECUTE 'ALTER TABLE ' || messages_table_name ||
            ' ADD CONSTRAINT ' || messages_chat_id_fk_name ||
            ' FOREIGN KEY (chat_id) REFERENCES private.bot_chats(chat_id)' ||
            ' ON DELETE CASCADE ON UPDATE CASCADE' ||
            ' ADD CONSTRAINT ' || messages_reply_to_fk_name ||
            ' FOREIGN KEY (reply_to) REFERENCES ' || messages_table_name || '(message_id)' ||
            ' ON DELETE SET NULL ON UPDATE CASCADE';

    EXECUTE 'CREATE TABLE ' || attachments_table_name ||
            ' PARTITION OF private.bot_messages_attachments FOR VALUES IN (''' ||
            initializing_chat_id::text || ''')';

    EXECUTE 'ALTER TABLE ' || attachments_table_name ||
            ' ADD CONSTRAINT ' || attachments_chat_id_message_id_fk_name ||
            ' FOREIGN KEY (chat_id, message_id) REFERENCES ' || messages_table_name || '(chat_id, message_id)' ||
            ' ON DELETE CASCADE ON UPDATE CASCADE' ||
            ' ADD CONSTRAINT ' || attachments_attachment_id_fk_name ||
            ' FOREIGN KEY (attachment_id) REFERENCES private.media(media_id)' ||
            ' ON UPDATE CASCADE';

    RETURN initializing_chat_id;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.send_bot_message(chat_id uuid, author uuid, message_text text, attachments media_file[], reply_to uuid DEFAULT NULL)
RETURNS uuid
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    sending_message_id uuid;
    adding_attachment media_file;
BEGIN
    SELECT private.update_user_online_status(author);

    UPDATE private.bot_chats
    SET was_in_chat = CURRENT_TIMESTAMP
    WHERE chat_id = send_bot_message.chat_id
      AND user_id = author;

    IF exists(
        SELECT 1
        FROM private.bot_chats bc
        WHERE user_id = author
          AND bc.chat_id = send_bot_message.chat_id
          AND NOT is_enabled
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF coalesce(trim(message_text), '') = ''
           AND (
               attachments IS NULL
                   OR array_length(attachments, 1) = 0
               ) THEN
        RAISE DATA_EXCEPTION;
    END IF;

    INSERT INTO private.bot_messages (message_id, chat_id, is_bot, message_text, sent_at, is_updated, updated_at, reply_to) VALUES
        (gen_random_uuid(), send_bot_message.chat_id, false,
         coalesce(trim(send_bot_message.message_text), ''), CURRENT_TIMESTAMP, false, null, send_bot_message.reply_to)
    RETURNING message_id INTO sending_message_id;

    IF adding_attachment IS NOT NULL THEN
        FOREACH adding_attachment IN ARRAY attachments LOOP
            INSERT INTO private.media (media_id, file_name, content_type)
            VALUES (adding_attachment.media_id, adding_attachment.file_name, adding_attachment.content_type);
            INSERT INTO private.bot_messages_attachments (attachment_id, message_id, chat_id)
            VALUES (adding_attachment.media_id, sending_message_id, send_bot_message.chat_id);
        END LOOP;
    END IF;

    RETURN sending_message_id;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.resend_to_bot_messages(chat_id uuid, author uuid, source_chat_type en_chat_type, source_chat_id uuid, messages_id uuid[])
RETURNS SETOF uuid
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    msg message;
    new_message_id uuid;
    message_map jsonb := '{}'::jsonb;
    new_reply_to uuid;
BEGIN
    SELECT private.update_user_online_status(author);

    UPDATE private.bot_chats
    SET was_in_chat = current_timestamp
    WHERE chat_id = resend_to_bot_messages.chat_id
      AND user_id = author;

    IF exists(
        SELECT 1
        FROM private.bot_chats bc
        WHERE user_id = author
          AND bc.chat_id = resend_to_bot_messages.chat_id
          AND NOT is_enabled
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    FOR msg IN
        SELECT gm.*
        FROM unnest(messages_id) AS mid
        CROSS JOIN LATERAL
            private.get_message(source_chat_id, mid, author, source_chat_type) gm
        ORDER BY gm.sent_at
    LOOP
        new_message_id := gen_random_uuid();

        IF msg.reply_to IS NOT NULL
           AND message_map ? msg.reply_to::text THEN
            new_reply_to :=
                (message_map ->> msg.reply_to::text)::uuid;
        ELSE
            new_reply_to := NULL;
        END IF;

        INSERT INTO private.bot_messages (message_id, chat_id, is_bot, message_text, sent_at, reply_to, resent_from, is_bot_resend, is_handled)
        VALUES (new_message_id, resend_to_bot_messages.chat_id,false, msg.message_text,
                current_timestamp,new_reply_to,msg.author, (source_chat_type = 'Bot'),false);

        message_map := message_map ||
            jsonb_build_object(msg.message_id::text, new_message_id::text);

        IF msg.attached_media IS NOT NULL THEN
            INSERT INTO private.bot_messages_attachments (attachment_id, message_id, chat_id)
            SELECT unnest(msg.attached_media),new_message_id, resend_to_bot_messages.chat_id;

            UPDATE private.media
            SET links_count = links_count + 1
            WHERE media_id IN (msg.attached_media);
        END IF;
        RETURN NEXT new_message_id;
    END LOOP;
    RETURN;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.disable_bot(chat_id uuid, disabling_by uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(disabling_by);

    UPDATE bot_chats
    SET is_enabled = false
    WHERE chat_id = disable_bot.chat_id
      AND user_id = disabling_by;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.enable_bot(chat_id uuid, enabling_by uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(enabling_by);

    UPDATE bot_chats
    SET is_enabled = true
    WHERE chat_id = enable_bot.chat_id
      AND user_id = enabling_by;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.delete_bot_chat(chat_id uuid, deleting_by uuid)
RETURNS int
SECURITY DEFINER
SET search_path = pg_catalog, private
AS
$$
DECLARE
    attachments_table_name text = 'private.bot_messages_attachments_' || replace(chat_id::text, '-', '_');
    messages_table_name text = 'private.bot_messages_' || replace(chat_id::text, '-', '_');
    affected_rows int;
BEGIN
    SELECT private.update_user_online_status(deleting_by);

    IF NOT exists(
        SELECT 1
        FROM private.bot_chats bc
        WHERE bc.chat_id = delete_bot_chat.chat_id
          AND bc.user_id = deleting_by
    ) THEN
        RAISE NO_DATA_FOUND;
    END IF;

    WITH deleted AS (
        DELETE
        FROM private.bot_messages_attachments bma
        WHERE bma.chat_id = delete_bot_chat.chat_id
        RETURNING attachment_id)

    UPDATE private.media
    SET links_count = links_count - 1
    WHERE media_id IN (
        SELECT attachment_id
        FROM deleted);

    EXECUTE 'DROP TABLE IF EXISTS ' || attachments_table_name;
    EXECUTE 'DROP TABLE IF EXISTS ' || messages_table_name;

    DELETE FROM private.bot_chats bc
    WHERE bc.chat_id = delete_bot_chat.chat_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;