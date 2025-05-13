using CabGroupService.Infrastructures.Repositories.GenericRepository;
using CabGroupService.Infrastructures.Repositories.Interfaces;
using CabGroupService.Models.Dtos;
using CabGroupService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace CabGroupService.Infrastructures.Repositories
{
    public class GroupRepository : GenericRepository<Group>, IGroupRepository
    {
        public GroupRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<ModelPagingResponse<Group>> GetGroupsWithPagination(PagingRequest pagingRequest)
        {
            ModelPagingResponse<Group> data = new ModelPagingResponse<Group>();
            data.Data = await _dbSet.OrderByDescending(x => x.CreatedAt).Skip((pagingRequest.PageNumber - 1) * pagingRequest.PageSize).Take(pagingRequest.PageSize).ToListAsync();
            data.Total = pagingRequest.PageSize == 0 ? 0 : (int)Math.Ceiling((double)_dbSet.Count() / pagingRequest.PageSize);
            return data;
        }
    }
}
