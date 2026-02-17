CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260216231119_Initial') THEN
    CREATE TABLE users (
        id uuid NOT NULL,
        username character varying(30) NOT NULL,
        display_name character varying(50) NOT NULL,
        created_at timestamp with time zone NOT NULL DEFAULT (now()),
        CONSTRAINT pk_users PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260216231119_Initial') THEN
    CREATE TABLE conversations (
        id uuid NOT NULL,
        type text NOT NULL,
        created_by uuid NOT NULL,
        created_at timestamp with time zone NOT NULL DEFAULT (now()),
        CONSTRAINT pk_conversations PRIMARY KEY (id),
        CONSTRAINT fk_conversations_users_created_by FOREIGN KEY (created_by) REFERENCES users (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260216231119_Initial') THEN
    CREATE TABLE conversation_members (
        conversation_id uuid NOT NULL,
        user_id uuid NOT NULL,
        joined_at timestamp with time zone NOT NULL DEFAULT (now()),
        last_read_at timestamp with time zone,
        CONSTRAINT pk_conversation_members PRIMARY KEY (conversation_id, user_id),
        CONSTRAINT fk_conversation_members_conversations_conversation_id FOREIGN KEY (conversation_id) REFERENCES conversations (id) ON DELETE CASCADE,
        CONSTRAINT fk_conversation_members_users_user_id FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260216231119_Initial') THEN
    CREATE TABLE messages (
        id uuid NOT NULL,
        conversation_id uuid NOT NULL,
        sender_id uuid NOT NULL,
        content_body character varying(4096) NOT NULL,
        content_media_url character varying(2048),
        content_thumbnail_url character varying(2048),
        content_file_name character varying(255),
        content_file_size_bytes bigint,
        content_media_duration_seconds integer,
        content_latitude double precision,
        content_longitude double precision,
        created_at timestamp with time zone NOT NULL DEFAULT (now()),
        sent_at timestamp with time zone,
        CONSTRAINT pk_messages PRIMARY KEY (id),
        CONSTRAINT fk_messages_conversations_conversation_id FOREIGN KEY (conversation_id) REFERENCES conversations (id) ON DELETE CASCADE,
        CONSTRAINT fk_messages_users_sender_id FOREIGN KEY (sender_id) REFERENCES users (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260216231119_Initial') THEN
    CREATE INDEX ix_conversation_members_user_id ON conversation_members (user_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260216231119_Initial') THEN
    CREATE INDEX ix_conversations_created_by ON conversations (created_by);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260216231119_Initial') THEN
    CREATE INDEX ix_messages_conversation_id ON messages (conversation_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260216231119_Initial') THEN
    CREATE INDEX ix_messages_sender_id ON messages (sender_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260216231119_Initial') THEN
    CREATE UNIQUE INDEX ix_users_username ON users (username);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20260216231119_Initial') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20260216231119_Initial', '10.0.3');
    END IF;
END $EF$;
COMMIT;

