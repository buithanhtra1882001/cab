namespace CabPostService.Infrastructures.Repositories.Interfaces
{
    public interface ISequenceGeneratorRepository
    {
        void Update(string tableName);
        long Get(string tableName);
    }
}
