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
            chat_id uuid,
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