using CabMediaService.Models.Entities;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace CabMediaService.Infrastructures.DbContexts
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
                     .AddContactPoint(host)
                     .WithPort(port)
                     .WithCredentials(username, password)
                     .Build();
            _session = cluster.Connect(keyspace);

            MappingConfiguration.Global.Define<MediaImageMappings>();
        }

        public Table<T> GetTable<T>()
        {
            return new Table<T>(_session);
        }
    }

    internal class MediaImageMappings : Mappings
    {
        public MediaImageMappings()
        {
            For<MediaImage>()
                .TableName("media_images")
                .PartitionKey(x => x.Id)
                .ClusteringKey(x => x.CreatedAt, SortOrder.Descending)
                .Column(p => p.CreatedBy, cm => cm.WithName("created_by"))
                .Column(p => p.FileName, cm => cm.WithName("file_name"))
                .Column(p => p.FilePath, cm => cm.WithName("file_path"))
                .Column(p => p.Size, cm => cm.WithName("size"))
                .Column(p => p.CreatedAt, cm => cm.WithName("created_at"))
                .Column(p => p.LastUsedAt, cm => cm.WithName("last_used_at"));
        }
    }
}