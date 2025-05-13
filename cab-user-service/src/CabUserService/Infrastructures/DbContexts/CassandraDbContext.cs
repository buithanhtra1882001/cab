using CabUserService.Models.Entities;
using CabUserService.Models.Entities.Base;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using Google.Protobuf.Collections;

namespace CabUserService.Infrastructures.DbContexts
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

            MappingConfiguration.Global.Define<UserFriendMappings>();
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

    internal class ChatMessageMappings : Mappings
    {
        public ChatMessageMappings()
        {
            For<ChatMessage>()
                .TableName("chat_messages")
                .PartitionKey(x => x.Id)
                .ClusteringKey(x => x.CreatedAt)
                .Column(p => p.Id, cm => cm.WithName("id"))
                .Column(p => p.SenderUserId, cm => cm.WithName("senderuser_id"))
                .Column(p => p.RecipientUserId, cm => cm.WithName("recipientuser_id"))
                .Column(p => p.Content, cm => cm.WithName("content"))
                .MapFromBaseEntity();
        }
    }

    internal class ChatMessageUserIdMaterializedViewMappings : Mappings
    {
        public ChatMessageUserIdMaterializedViewMappings()
        {
            For<ChatMessageUserIdMaterializedView>()
                .TableName("chat_messages_userId")
                .PartitionKey(x => x.SenderUserId)
                .ClusteringKey(x => x.CreatedAt)
                .Column(p => p.Id, cm => cm.WithName("id"))
                .Column(p => p.SenderUserId, cm => cm.WithName("senderuser_id"))
                .Column(p => p.RecipientUserId, cm => cm.WithName("recipientuser_id"))
                .Column(p => p.Content, cm => cm.WithName("content"))
                .MapFromBaseEntity();
        }
    }

    internal class UserFriendMappings : Mappings
    {
        public UserFriendMappings()
        {
            For<UserFriend>()
               .TableName("user_friends")
               .PartitionKey(x => x.UserId)
               .ClusteringKey(x => x.CreatedAt)
               .Column(p => p.UserId, cm => cm.WithName("user_id"))
               .Column(p => p.FriendId, cm => cm.WithName("friend_id"))
               .MapFromBaseEntity();
        }
    }

    internal class UserImageMappings : Mappings
    {
        public UserImageMappings()
        {
            For<UserImage>()
               .TableName("user_images")
               .PartitionKey(x => x.UserId)
               .ClusteringKey(x => x.CreatedAt)
               .Column(p => p.UserId, cm => cm.WithName("user_id"))
               .Column(p => p.Url, cm => cm.WithName("url"))
               .Column(p => p.Size, cm => cm.WithName("size"))
               .MapFromBaseEntity();
        }
    }
}