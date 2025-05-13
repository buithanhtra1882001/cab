using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Interfaces;

namespace CabPostService.Infrastructures.Repositories
{
    public class SequenceGeneratorRepository : ISequenceGeneratorRepository
    {
        private readonly Cassandra.ISession _session;
        public SequenceGeneratorRepository(ScyllaDbContext context)
        {
            _session = context._session;
        }

        public long Get(string tableName)
        {
            string cqlQuery = $"select sequence_number from sequence_generator where table_name = '{tableName}';";

            var session = _session.Execute(cqlQuery).FirstOrDefault();

            if (session is null)
                return 0;

            var sequenceNumber = session.GetValue<long>(0);
            return sequenceNumber;
        }

        public void Update(string tableName)
        {
            string cqlQuery = $"update sequence_generator set sequence_number = sequence_number + 1 where table_name = '{tableName}';";
            _session.Execute(cqlQuery);
        }
    }
}
