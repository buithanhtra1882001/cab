using CabUserService.Infrastructures.Repositories.Base;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.Models.Entities;

namespace CabUserService.Infrastructures.Repositories
{
    public class DonateReceiverRequestRepository : PostgresBaseRepository, IDonateReceiverRequestRepository
    {
        public DonateReceiverRequestRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<int> HardDeleteAsync<IdType>(IdType id)
        {
            return 0;
        }

        public async Task<DonateReceiverRequest> GetByIdAsync<IdType>(IdType id)
        {
            return null;
        }

        public async Task<int> CreateAsync(DonateReceiverRequest entity)
        {
            return 0;
        }

        public async Task<int> UpdateAsync(DonateReceiverRequest entity)
        {
            return 0;
        }
    }
}
