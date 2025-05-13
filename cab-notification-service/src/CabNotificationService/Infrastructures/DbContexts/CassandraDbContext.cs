using CabNotificationService.Models.Entities;
using CabNotificationService.Models.Entities.Base;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace CabNotificationService.Infrastructures.DbContexts
{
    public class CassandraDbContext
    {
        public readonly Cassandra.ISession _session;

        public CassandraDbContext(IConfiguration configuration, ILogger<CassandraDbContext> logger)
        {
            var host = configuration.GetValue<string>("Cassandra:Host");
            var port = configuration.GetValue<int>("Cassandra:Port");
            var username = configuration.GetValue<string>("Cassandra:Username");
            var password = configuration.GetValue<string>("Cassandra:Password");
            var keyspace = configuration.GetValue<string>("Cassandra:Keyspace");

            var cluster = Cluster.Builder()
                     //.AddContactPoints(new IPEndPoint(IPAddress.Parse(host), port))
                     .AddContactPoint(host)
                     .WithPort(port)
                     .WithCredentials(username, password)
                     .Build();
            _session = cluster.Connect(keyspace);
        }

        public Table<T> GetTable<T>()
        {
            return new Table<T>(_session);
        }
    }

    internal static class GenericMappings
    {
        public static Map<T> MapFromBaseEntity<T>(this Map<T> mapper) where T : BaseEntity
        {
            return mapper.Column(p => p.CreatedAt, cm => cm.WithName("created_at"))
               .Column(p => p.UpdatedAt, cm => cm.WithName("updated_at"));
        }
    }

    internal class NotificationMappings : Mappings
    {
        public NotificationMappings()
        {
            For<Notification>()
                .TableName("notifications")
                .PartitionKey(x => x.Id)
                .ClusteringKey(x => x.CreatedAt)
                .Column(p => p.Id, cm => cm.WithName("id"))
                .Column(p => p.UserId, cm => cm.WithName("user_Id"))
                .Column(p => p.ActorId, cm => cm.WithName("actor_Id"))
                .Column(p => p.ReferenceId, cm => cm.WithName("reference_Id"))
                .Column(p => p.Message, cm => cm.WithName("message"))
                .Column(p => p.NotificationType, cm => cm.WithName("notification_type"))
                .Column(p => p.IsRead, cm => cm.WithName("is_read"))
                .Column(p => p.ReferenceUrl, cm => cm.WithName("reference_Url"))
                .Column(p => p.Type, cm => cm.WithName("type"))
                .MapFromBaseEntity();
        }
    }

    internal class NotificationUserIdMaterializedViewMappings : Mappings
    {
        public NotificationUserIdMaterializedViewMappings()
        {
            For<NotificationUserIdMaterializedView>()
                .TableName("notifications_userId")
                .PartitionKey(x => x.UserId)
                .ClusteringKey(x => x.CreatedAt)
                .Column(p => p.Id, cm => cm.WithName("id"))
                .Column(p => p.UserId, cm => cm.WithName("user_Id"))
                .Column(p => p.ActorId, cm => cm.WithName("actor_Id"))
                .Column(p => p.ReferenceId, cm => cm.WithName("reference_Id"))
                .Column(p => p.Message, cm => cm.WithName("message"))
                .Column(p => p.NotificationType, cm => cm.WithName("notification_type"))
                .Column(p => p.IsRead, cm => cm.WithName("is_read"))
                .Column(p => p.ReferenceUrl, cm => cm.WithName("reference_Url"))
                .Column(p => p.Type, cm => cm.WithName("type"))
                .MapFromBaseEntity();
        }
    }
}