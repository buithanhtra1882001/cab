using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using CabPostService.Models.Queries;

namespace CabPostService.Infrastructures.Repositories.Interfaces
{
    public interface IPostCommentPostIdMaterializedViewRepository :
        IBaseRepository<PostCommentPostIdMaterializedView>
    {
    }
}
