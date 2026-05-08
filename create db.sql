DO
$$
BEGIN
    IF NOT exists(SELECT 1 FROM pg_database WHERE datname = 'messenger_db') THEN
        CREATE DATABASE messenger_db;
    END IF;
END;
$$;