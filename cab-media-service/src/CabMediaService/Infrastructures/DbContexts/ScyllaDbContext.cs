using CabMediaService.Models.Entities;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace CabMediaService.Infrastructures.DbContexts
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
                     .AddContactPoint(host)
                     .WithPort(port)
                     //.WithCredentials(username, password)
                     .Build();
            _session = cluster.Connect(keyspace);

            MappingConfiguration.Global.Define<MediaImageMappings>();
        }

        public Table<T> GetTable<T>()
        {
            return new Table<T>(_session);
        }
    }
}