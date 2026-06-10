DROP FUNCTION IF EXISTS sch_user.send_personal_message(chat_id uuid, author uuid, message_text text, attachments media_file[], reply_to uuid);
DROP FUNCTION IF EXISTS sch_user.send_public_message(chat_id uuid, author uuid, message_text text, attachments media_file[], reply_to uuid);
DROP FUNCTION IF EXISTS sch_user.send_bot_message(chat_id uuid, author uuid, message_text text, attachments media_file[], reply_to uuid);

CREATE OR REPLACE FUNCTION sch_user.check_personal_message_send_ability(chat_id uuid, author uuid, reply_to uuid DEFAULT NULL)
RETURNS bool
SECURITY DEFINER
SET search_path = sch_user, public, private
AS
$$
BEGIN
    SELECT private.update_user_online_status(author);

    IF NOT exists(
        SELECT 1
        FROM private.personal_chats_members pcm
        WHERE pcm.chat_id = check_personal_message_send_ability.chat_id
          AND pcm.user_id = author
    ) THEN
        RAISE EXCEPTION 'Chat not found'
        USING ERRCODE = '23514';
    END IF;

    IF exists(
        SELECT 1
        FROM private.personal_chats_members pcm1
        JOIN private.personal_chats_members pcm2 ON pcm1.chat_id = pcm2.chat_id
                                                AND coalesce(pcm1.user_id != pcm2.user_id, true)
        WHERE pcm1.user_id = author
          AND pcm2.user_id IS NULL
          AND pcm1.chat_id = check_personal_message_send_ability.chat_id
    ) THEN
        RAISE EXCEPTION 'Second user deleted'
        USING ERRCODE = '23514';
    end if;

    UPDATE private.personal_chats_members
    SET was_in_chat = CURRENT_TIMESTAMP
    WHERE chat_id = check_personal_message_send_ability.chat_id
      AND user_id = author;

    IF exists(
        SELECT 1
        FROM private.personal_chats_members pcm1
        JOIN private.personal_chats_members pcm2 ON pcm1.chat_id = pcm2.chat_id
                                                AND coalesce(pcm1.user_id != pcm2.user_id, false)
        JOIN private.users_blocks ub ON ub.user_id = pcm1.user_id AND ub.block_by = pcm2.user_id
        WHERE pcm1.chat_id = check_personal_message_send_ability.chat_id
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF reply_to IS NOT NULL AND NOT exists(
        SELECT 1
        FROM private.personal_messages pm
        WHERE check_personal_message_send_ability.chat_id = pm.chat_id
          AND message_id = check_personal_message_send_ability.reply_to
    ) THEN
        RAISE EXCEPTION 'Replying message not found'
        USING ERRCODE = '23514';
    end if;

    RETURN true;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.check_public_message_send_ability(chat_id uuid, author uuid, reply_to uuid DEFAULT NULL)
RETURNS bool
SECURITY DEFINER
SET search_path = sch_user, public, private
AS
$$
BEGIN
    SELECT private.update_user_online_status(author);

    IF NOT exists(
        SELECT 1
        FROM private.public_chats_members pcm
        WHERE pcm.chat_id = check_public_message_send_ability.chat_id
          AND pcm.user_id = author
    ) THEN
        RAISE EXCEPTION 'Chat not found'
        USING ERRCODE = '23514';
    END IF;

    UPDATE private.public_chats_members
    SET was_in_chat = CURRENT_TIMESTAMP
    WHERE chat_id = check_public_message_send_ability.chat_id
      AND user_id = author;

    IF exists(
        SELECT 1
        FROM private.public_chats_banned_users pcbu
        WHERE pcbu.user_id = author
          AND pcbu.chat_id = check_public_message_send_ability.chat_id
    ) THEN
        RAISE EXCEPTION 'User is banned in chat'
            USING ERRCODE = '42501';
    END IF;

    IF exists(
        SELECT 1
        FROM private.public_chats_members pcm
        WHERE pcm.chat_id = check_public_message_send_ability.chat_id
          AND pcm.user_id = author
          AND pcm.role = 'Reader'
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF reply_to IS NOT NULL AND NOT exists(
        SELECT 1
        FROM private.public_messages pm
        WHERE check_public_message_send_ability.chat_id = pm.chat_id
          AND message_id = check_public_message_send_ability.reply_to
    ) THEN
        RAISE EXCEPTION 'Replying message not found'
        USING ERRCODE = '23514';
    end if;

    RETURN true;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.check_bot_message_send_ability(chat_id uuid, author uuid, reply_to uuid DEFAULT NULL)
RETURNS bool
SECURITY DEFINER
SET search_path = sch_user, public, private
AS
$$
BEGIN
    SELECT private.update_user_online_status(author);

    IF NOT exists(
        SELECT 1
        FROM private.bot_chats bcm
        WHERE bcm.chat_id = check_bot_message_send_ability.chat_id
          AND bcm.user_id = author
    ) THEN
        RAISE EXCEPTION 'Chat not found'
        USING ERRCODE = '23514';
    END IF;

    IF exists(
        SELECT 1
        FROM private.bot_chats bc
        WHERE bc.chat_id = check_bot_message_send_ability.chat_id
          AND bc.bot_id IS NULL
    ) THEN
        RAISE EXCEPTION 'Bot deleted'
        USING ERRCODE = '23514';
    end if;

    UPDATE private.bot_chats
    SET was_in_chat = CURRENT_TIMESTAMP
    WHERE chat_id = check_bot_message_send_ability.chat_id
      AND user_id = author;

    IF exists(
        SELECT 1
        FROM private.bot_chats bc
        WHERE bc.chat_id = check_bot_message_send_ability.chat_id
          AND NOT bc.is_enabled
    ) THEN
        RAISE INSUFFICIENT_PRIVILEGE;
    END IF;

    IF reply_to IS NOT NULL AND NOT exists(
        SELECT 1
        FROM private.bot_messages pm
        WHERE check_bot_message_send_ability.chat_id = pm.chat_id
          AND message_id = check_bot_message_send_ability.reply_to
    ) THEN
        RAISE EXCEPTION 'Replying message not found'
        USING ERRCODE = '23514';
    end if;

    RETURN true;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.check_access_to_message(chat_type en_chat_type, chat_id uuid, user_id uuid, message_id uuid)
RETURNS bool
SECURITY DEFINER
SET search_path = sch_user, public, private
AS
$$
DECLARE
    deleting_message private.public_messages;
BEGIN
    CASE chat_type
        WHEN 'Public'::en_chat_type THEN
            IF exists(
                SELECT 1
                FROM private.public_chats_banned_users pcbu
                WHERE pcbu.chat_id = check_access_to_message.chat_id
                  AND pcbu.user_id = check_access_to_message.user_id
            ) THEN
                RAISE EXCEPTION 'User is banned in chat'
                USING ERRCODE = '42501';
            END IF;

            SELECT * INTO deleting_message
            FROM private.public_messages
            WHERE private.public_messages.chat_id = check_access_to_message.chat_id
              AND private.public_messages.message_id = check_access_to_message.message_id;

            IF deleting_message.author != check_access_to_message.user_id AND
               (SELECT coalesce(role::text, '')
                FROM private.public_chats_members
                WHERE public_chats_members.chat_id = check_access_to_message.chat_id
                  AND public_chats_members.user_id = check_access_to_message.user_id) NOT IN ('Creator', 'Administrator') THEN
                RAISE EXCEPTION 'You cannot delete this public message.' USING ERRCODE = '42501';
            END IF;

            IF exists(
                SELECT 1
                FROM private.public_chats_members pcm
                WHERE pcm.user_id = check_access_to_message.user_id
                AND pcm.chat_id = check_access_to_message.chat_id
                AND pcm.role = 'Reader'
            ) THEN
                RAISE EXCEPTION 'Readers cannot perform this action in the public chat.' USING ERRCODE = '42501';
            END IF;
        WHEN 'Personal'::en_chat_type THEN
            IF NOT exists(
                SELECT 1
                FROM private.personal_messages
                WHERE personal_messages.chat_id = check_access_to_message.chat_id
                  AND personal_messages.message_id = check_access_to_message.message_id
                  AND personal_messages.author = user_id
            ) THEN
                RAISE EXCEPTION 'Message not found'
                USING ERRCODE = '23514';
            end if;
        WHEN 'Bot'::en_chat_type THEN
            IF NOT exists(
                SELECT 1
                FROM private.bot_messages bm
                JOIN private.bot_chats bc ON bc.chat_id = bm.chat_id
                WHERE bm.chat_id = check_access_to_message.chat_id
                  AND bm.message_id = check_access_to_message.message_id
                  AND bc.user_id = check_access_to_message.user_id
            ) THEN
                RAISE EXCEPTION 'Message not found'
                USING ERRCODE = '23514';
            end if;
        ELSE RAISE DATATYPE_MISMATCH;
    END CASE;

    PERFORM private.update_user_online_status(user_id);

    RETURN true;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sch_user.check_access_to_attachment(chat_type en_chat_type, chat_id uuid, message_id uuid, attachment_id uuid)
RETURNS bool
SECURITY DEFINER
SET search_path = sch_user, public, private
AS
$$
BEGIN
    CASE chat_type
        WHEN 'Public'::en_chat_type THEN
            IF NOT exists(
                SELECT 1
                FROM private.public_messages_attachments pma
                WHERE pma.chat_id = check_access_to_attachment.chat_id
                  AND pma.message_id = check_access_to_attachment.message_id
                  AND pma.attachment_id = check_access_to_attachment.attachment_id
            ) THEN
                RAISE EXCEPTION 'File not found'
                USING ERRCODE = '23514';
            end if;

            IF exists(
                SELECT 1
                FROM private.public_messages pm
                WHERE pm.chat_id = check_access_to_attachment.chat_id
                  AND pm.message_id = check_access_to_attachment.message_id
                  AND pm.resent_from IS NOT NULL
            ) THEN
                RAISE EXCEPTION 'Attachments cannot be removed from forwarded messages.'
                USING ERRCODE = '42501';
            end if;
        WHEN 'Personal'::en_chat_type THEN
            IF NOT exists(
                SELECT 1
                FROM private.personal_messages_attachments pma
                WHERE pma.chat_id = check_access_to_attachment.chat_id
                  AND pma.message_id = check_access_to_attachment.message_id
                  AND pma.attachment_id = check_access_to_attachment.attachment_id
            ) THEN
                RAISE EXCEPTION 'File not found'
                USING ERRCODE = '23514';
            end if;

            IF exists(
                SELECT 1
                FROM private.personal_messages pm
                WHERE pm.chat_id = check_access_to_attachment.chat_id
                  AND pm.message_id = check_access_to_attachment.message_id
                  AND pm.resent_from IS NOT NULL
            ) THEN
                RAISE EXCEPTION 'Attachments cannot be removed from forwarded messages.'
                USING ERRCODE = '42501';
            end if;
        WHEN 'Bot'::en_chat_type THEN
            IF NOT exists(
                SELECT 1
                FROM private.bot_messages_attachments bma
                WHERE bma.chat_id = check_access_to_attachment.chat_id
                  AND bma.message_id = check_access_to_attachment.message_id
                  AND bma.attachment_id = check_access_to_attachment.attachment_id
            ) THEN
                RAISE EXCEPTION 'File not found'
                USING ERRCODE = '23514';
            end if;

            IF exists(
                SELECT 1
                FROM private.bot_messages bm
                WHERE bm.chat_id = check_access_to_attachment.chat_id
                  AND bm.message_id = check_access_to_attachment.message_id
                  AND bm.resent_from IS NOT NULL
            ) THEN
                RAISE EXCEPTION 'Attachments cannot be removed from forwarded messages.'
                USING ERRCODE = '42501';
            end if;
        ELSE RAISE DATATYPE_MISMATCH;
    END CASE;

    RETURN true;
END;
$$
LANGUAGE plpgsql;

DROP FUNCTION IF EXISTS sch_user.resend_to_bot_messages(chat_id uuid, author uuid, source_chat_type en_chat_type, source_chat_id uuid, messages_id uuid[]);
DROP FUNCTION IF EXISTS sch_user.resend_to_private_messages(chat_id uuid, author uuid, source_chat_type en_chat_type, source_chat_id uuid, messages_id uuid[]);
DROP FUNCTION IF EXISTS sch_user.resend_to_public_messages(chat_id uuid, author uuid, source_chat_type en_chat_type, source_chat_id uuid, messages_id uuid[]);

CREATE OR REPLACE FUNCTION sch_user.get_messages_by_id(chat_id uuid, message_ids uuid[], getting_by uuid, chat_type en_chat_type)
RETURNS SETOF message
SECURITY DEFINER
SET SEARCH_PATH = sch_user, public, private
AS
$$
DECLARE
    message_id uuid;
    message message;
BEGIN
    FOREACH message_id IN ARRAY message_ids LOOP
        message = private.get_message(chat_id, message_id, getting_by, chat_type);
        IF message IS NOT NULL THEN RETURN NEXT message;
        END IF;
    END LOOP;
END;
$$
LANGUAGE plpgsql;

ALTER FUNCTION sch_user.delete_file_from_public_message(chat_id uuid, attachment_id uuid, deleting_by uuid)
SET SCHEMA private;
ALTER FUNCTION sch_user.delete_file_from_personal_message(chat_id uuid, attachment_id uuid, deleting_by uuid)
SET SCHEMA private;
ALTER FUNCTION sch_user.delete_personal_message(chat_id uuid, message_id uuid, deleting_by uuid)
SET SCHEMA private;
ALTER FUNCTION sch_user.delete_public_message(chat_id uuid, message_id uuid, deleting_by uuid)
SET SCHEMA private;

DO
$$
BEGIN
    IF NOT exists(SELECT 1 FROM pg_type WHERE typname = 'message_input') THEN
        CREATE TYPE message_input AS
        (
            message_id uuid,
            author uuid,
            is_bot bool,
            message_text text,
            sent_at timestamp,
            is_updated boolean,
            updated_at timestamp,
            reply_to uuid,
            resent_from uuid,
            is_bot_resend bool,
            attached_media media_file[]
        );
    END IF;
END;
$$;

CREATE OR REPLACE FUNCTION private.save_message(chat_type en_chat_type, chat_id uuid, message message_input)
RETURNS int
SECURITY DEFINER
AS
$$
DECLARE
    affected_rows int;
    attachment media_file;
BEGIN
    CASE chat_type
        WHEN 'Public'::en_chat_type THEN
            IF message IS NOT NULL THEN
                INSERT INTO private.public_messages
                    (message_id, chat_id, author, message_text, sent_at, is_updated, updated_at, reply_to, resent_from, is_bot_resend)
                VALUES (message.message_id, save_message.chat_id, message.author, message.message_text, message.sent_at, message.is_updated, message.updated_at, message.reply_to, message.resent_from, message.is_bot_resend);

                FOREACH attachment IN ARRAY message.attached_media LOOP
                    IF attachment IS NOT NULL THEN
                        IF NOT exists(
                            SELECT 1
                            FROM private.media
                            WHERE media_id = attachment.media_id
                        ) THEN
                            INSERT INTO private.media (media_id, file_name, content_type)
                            VALUES (attachment.media_id, attachment.file_name, attachment.content_type);
                        ELSE
                            UPDATE private.media
                            SET links_count = links_count + 1
                            WHERE media_id = attachment.media_id;
                        end if;

                        INSERT INTO private.public_messages_attachments (attachment_id, message_id, chat_id)
                        VALUES (attachment.media_id, message.message_id, save_message.chat_id);
                    end if;
                end loop;
            end if;

        WHEN 'Personal'::en_chat_type THEN
            IF message IS NOT NULL THEN
                INSERT INTO private.personal_messages
                    (message_id, chat_id, author, message_text, sent_at, is_updated, updated_at, reply_to, resent_from, is_bot_resend)
                VALUES (message.message_id, save_message.chat_id, message.author, message.message_text, message.sent_at, message.is_updated, message.updated_at, message.reply_to, message.resent_from, message.is_bot_resend);

                FOREACH attachment IN ARRAY message.attached_media LOOP
                    IF attachment IS NOT NULL THEN
                        IF NOT exists(
                            SELECT 1
                            FROM private.media
                            WHERE media_id = attachment.media_id
                        ) THEN
                            INSERT INTO private.media (media_id, file_name, content_type)
                            VALUES (attachment.media_id, attachment.file_name, attachment.content_type);
                        ELSE
                            UPDATE private.media
                            SET links_count = links_count + 1
                            WHERE media_id = attachment.media_id;
                        end if;

                        INSERT INTO private.personal_messages_attachments (attachment_id, message_id, chat_id)
                        VALUES (attachment.media_id, message.message_id, save_message.chat_id);
                    end if;
                end loop;
            end if;

        WHEN 'Bot'::en_chat_type THEN
            IF message IS NOT NULL THEN
                INSERT INTO private.bot_messages
                    (message_id, chat_id, is_bot, message_text, sent_at, is_updated, updated_at, reply_to, resent_from, is_bot_resend)
                VALUES (message.message_id, save_message.chat_id, message.is_bot, message.message_text, message.sent_at, message.is_updated, message.updated_at, message.reply_to, message.resent_from, message.is_bot_resend);

                FOREACH attachment IN ARRAY message.attached_media LOOP
                    IF attachment IS NOT NULL THEN
                        IF NOT exists(
                            SELECT 1
                            FROM private.media
                            WHERE media_id = attachment.media_id
                        ) THEN
                            INSERT INTO private.media (media_id, file_name, content_type)
                            VALUES (attachment.media_id, attachment.file_name, attachment.content_type);
                        ELSE
                            UPDATE private.media
                            SET links_count = links_count + 1
                            WHERE media_id = attachment.media_id;
                        end if;

                        INSERT INTO private.bot_messages_attachments (attachment_id, message_id, chat_id)
                        VALUES (attachment.media_id, message.message_id, save_message.chat_id);
                    end if;
                end loop;
            end if;

        ELSE RAISE DATATYPE_MISMATCH;
    END CASE;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION private.update_message_text(chat_type en_chat_type, chat_id uuid, message_id uuid, new_message_text text, updated_at timestamp)
RETURNS int
SECURITY DEFINER
AS
$$
DECLARE
    affected_rows int;
BEGIN
    CASE chat_type
        WHEN 'Public'::en_chat_type THEN
            UPDATE private.public_messages pm
            SET message_text = new_message_text,
                updated_at = update_message_text.updated_at,
                is_updated = true
            WHERE pm.chat_id = update_message_text.chat_id
              AND pm.message_id = update_message_text.message_id;

        WHEN 'Personal'::en_chat_type THEN
            UPDATE private.personal_messages pm
            SET message_text = new_message_text,
                updated_at = update_message_text.updated_at,
                is_updated = true
            WHERE pm.chat_id = update_message_text.chat_id
              AND pm.message_id = update_message_text.message_id;

        WHEN 'Bot'::en_chat_type THEN
            UPDATE private.bot_messages bm
            SET message_text = new_message_text,
                updated_at = update_message_text.updated_at,
                is_updated = true
            WHERE bm.chat_id = update_message_text.chat_id
              AND bm.message_id = update_message_text.message_id;

        ELSE RAISE DATATYPE_MISMATCH;
    END CASE;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION private.delete_message(chat_type en_chat_type, chat_id uuid, message_id uuid)
RETURNS int
SECURITY DEFINER
AS
$$
DECLARE
    affected_rows int;
BEGIN
    CASE chat_type
        WHEN 'Public'::en_chat_type THEN
            DELETE FROM private.public_messages pm
            WHERE pm.chat_id = delete_message.chat_id
              AND pm.message_id = delete_message.message_id;

        WHEN 'Personal'::en_chat_type THEN
            DELETE FROM private.personal_messages pm
            WHERE pm.chat_id = delete_message.chat_id
              AND pm.message_id = delete_message.message_id;

        WHEN 'Bot'::en_chat_type THEN
            DELETE FROM private.bot_messages bm
            WHERE bm.chat_id = delete_message.chat_id
              AND bm.message_id = delete_message.message_id;

        ELSE RAISE DATATYPE_MISMATCH;
    END CASE;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION private.delete_attachment(chat_type en_chat_type, chat_id uuid, media_id uuid)
RETURNS int
SECURITY DEFINER
AS
$$
DECLARE
    affected_rows int;
BEGIN
    CASE chat_type
        WHEN 'Public'::en_chat_type THEN
            DELETE FROM private.public_messages_attachments pma
            WHERE pma.chat_id = delete_attachment.chat_id
              AND pma.message_id = delete_attachment.media_id;

        WHEN 'Personal'::en_chat_type THEN
            DELETE FROM private.personal_messages_attachments pma
            WHERE pma.chat_id = delete_attachment.chat_id
              AND pma.message_id = delete_attachment.media_id;

        WHEN 'Bot'::en_chat_type THEN
            DELETE FROM private.bot_messages_attachments bma
            WHERE bma.chat_id = delete_attachment.chat_id
              AND bma.message_id = delete_attachment.media_id;

        ELSE RAISE DATATYPE_MISMATCH;
    END CASE;
    
    UPDATE private.media m
    SET links_count = links_count - 1
    WHERE m.media_id = delete_attachment.media_id;

    GET DIAGNOSTICS affected_rows = ROW_COUNT;
    RETURN affected_rows;
END;
$$
LANGUAGE plpgsql;