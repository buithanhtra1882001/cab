using CabPaymentService.Model.Entities;
using CabPaymentService.Model.Entities.Base;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace CabPaymentService.Infrastructures.DbContexts
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

            MappingConfiguration.Global.Define<VNPAYTransactionMappings>();
            MappingConfiguration.Global.Define<VNPAYTransactionDetailMappings>();
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
            return mapper
               .Column(p => p.CreatedAt, cm => cm.WithName("created_at"))
               .Column(p => p.UpdatedAt, cm => cm.WithName("updated_at"))
               .Column(p => p.DeletedAt, cm => cm.WithName("deleted_at"))

               .Column(p => p.CreatedAt, cm => cm.WithName("created_by"))
               .Column(p => p.DeletedBy, cm => cm.WithName("deleted_by"))
               .Column(p => p.UpdatedBy, cm => cm.WithName("updated_by"))

               .Column(p => p.IsActive, cm => cm.WithName("is_active"))
               .Column(p => p.IsDeleted, cm => cm.WithName("is_delete"));
        }
    }

    internal class VNPAYTransactionMappings : Mappings
    {
        public VNPAYTransactionMappings()
        {
            For<VnPayTransaction>()
               .TableName("vnpay_transaction");
            //For<VNPAYTransaction>()
            //   .TableName("vnpay_transaction")
            //   .PartitionKey(x => x.UserId)
            //   .ClusteringKey(x => x.CreatedAt)
            //   .Column(p => p.UserId, cm => cm.WithName("user_id"))
            //   .Column(p => p.Username, cm => cm.WithName("username"))
            //   .Column(p => p.Email, cm => cm.WithName("email"))
            //   .Column(p => p.Fullname, cm => cm.WithName("full_name"))
            //   .Column(p => p.IdentityCardNumber, cm => cm.WithName("identity_card_number"))
            //   .Column(p => p.Dob, cm => cm.WithName("dob"))
            //   .Column(p => p.City, cm => cm.WithName("city"))
            //   .Column(p => p.Phone, cm => cm.WithName("phone"))
            //   .Column(p => p.Avatar, cm => cm.WithName("avatar"))
            //   .Column(p => p.Sex, cm => cm.WithName("sex"))
            //   .Column(p => p.Description, cm => cm.WithName("description"))
            //   .MapFromBaseEntity();
        }
    }

    internal class VNPAYTransactionDetailMappings : Mappings
    {
        public VNPAYTransactionDetailMappings()
        {
            For<VnpayTransactionDetail>()
               .TableName("vnpay_transaction_detail");
        }
    }
}
