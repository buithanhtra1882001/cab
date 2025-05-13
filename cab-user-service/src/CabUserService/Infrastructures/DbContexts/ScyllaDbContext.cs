using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace CabUserService.Infrastructures.DbContexts
{
    public class ScyllaDbContext
    {
        public readonly Cassandra.ISession _session;

        public ScyllaDbContext(IConfiguration configuration, ILogger<ScyllaDbContext> logger)
        {
            var host = configuration.GetValue<string>("Scylladb:Host");
            var port = configuration.GetValue<int>("Scylladb:Port");
            var username = configuration.GetValue<string>("Scylladb:Username");
            var password = configuration.GetValue<string>("Scylladb:Password");
            var keyspace = configuration.GetValue<string>("Scylladb:Keyspace");

            var cluster = Cluster.Builder()
                     //.AddContactPoints(new IPEndPoint(IPAddress.Parse(host), port))
                     .AddContactPoint(host)
                     .WithPort(port)
                     //.WithCredentials(username, password)
                     .Build();
            _session = cluster.Connect(keyspace);

            MappingConfiguration.Global.Define<UserFriendMappings>();
            MappingConfiguration.Global.Define<ChatMessageMappings>();
            MappingConfiguration.Global.Define<UserImageMappings>();
            MappingConfiguration.Global.Define<ChatMessageUserIdMaterializedViewMappings>();
        }

        public Table<T> GetTable<T>()
        {
            return new Table<T>(_session);
        }
    }
}
