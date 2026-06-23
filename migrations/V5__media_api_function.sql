CREATE SCHEMA IF NOT EXISTS sch_media;

DO
$$
BEGIN
    IF NOT exists(SELECT 1 FROM pg_catalog.pg_roles WHERE rolname = 'media_web_api') THEN
        CREATE ROLE media_web_api WITH
        LOGIN
        PASSWORD 'media_web_api_password';
    END IF;
END
$$;

REVOKE ALL ON SCHEMA public FROM media_web_api;
GRANT USAGE ON SCHEMA public TO media_web_api;
REVOKE ALL ON ALL TABLES IN SCHEMA public FROM media_web_api;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA public FROM media_web_api;
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA public FROM media_web_api;

REVOKE ALL ON SCHEMA private FROM media_web_api;
REVOKE ALL ON ALL TABLES IN SCHEMA private FROM media_web_api;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA private FROM media_web_api;
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA private FROM media_web_api;

REVOKE ALL ON SCHEMA sch_media FROM media_web_api;
GRANT USAGE ON SCHEMA sch_media TO media_web_api;

REVOKE ALL ON ALL TABLES IN SCHEMA sch_media FROM media_web_api;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA sch_media FROM media_web_api;

REVOKE ALL ON ALL FUNCTIONS IN SCHEMA sch_media FROM media_web_api;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA sch_media TO media_web_api;

CREATE OR REPLACE FUNCTION sch_media.get_media_by_id(media_id uuid)
RETURNS media_file
SECURITY DEFINER
SET search_path = sch_media, public, private
AS
$$
    SELECT (m.media_id, file_name, content_type) :: media_file
    FROM private.media m
    WHERE m.media_id = get_media_by_id.media_id
    LIMIT 1;
$$
LANGUAGE sql;