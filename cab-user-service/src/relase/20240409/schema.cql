USE userdb;

CREATE TABLE IF NOT EXISTS chat_messages (
    id uuid, 
    senderuser_id uuid, 
    recipientuser_id uuid, 
    content text, 
    created_at timestamp, 
    updated_at timestamp, 
    PRIMARY KEY ((senderuser_id, id), created_at)
)
WITH CLUSTERING ORDER BY (created_at DESC);

CREATE MATERIALIZED VIEW chat_messages_userId
AS SELECT *
FROM chat_messages
WHERE senderuser_id IS NOT NULL AND recipientuser_id IS NOT NULL AND id IS NOT NULL AND created_at IS NOT NULL
PRIMARY KEY ((recipientuser_id), senderuser_id, created_at, id)
WITH CLUSTERING ORDER BY (created_at DESC);
