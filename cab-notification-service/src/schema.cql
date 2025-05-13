CREATE KEYSPACE IF NOT EXISTS notificationdb WITH replication = {'class': 'SimpleStrategy', 'replication_factor': '1'};

USE notificationdb;

CREATE TABLE IF NOT EXISTS notifications (
    id uuid,
    user_Id uuid,
    actor_Id uuid,
    reference_Id uuid,
    message text,
    notification_type text,
    created_at timestamp,
    updated_at timestamp,
    is_read boolean,
    reference_Url text,
    type text,
    PRIMARY KEY (id, created_at)
)
WITH CLUSTERING ORDER BY (created_at DESC);

CREATE MATERIALIZED VIEW notifications_userId
AS SELECT *
FROM notifications
WHERE user_Id IS NOT NULL AND id IS NOT NULL AND created_at IS NOT NULL
PRIMARY KEY (user_Id, created_at, id)
WITH CLUSTERING ORDER BY (created_at DESC);