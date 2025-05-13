using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using CabPostService.Models.Queries;

namespace CabPostService.Infrastructures.Repositories.Interfaces
{
    public interface IPostRepository : IPostgresBaseRepository<Post>
    {
        Task<List<Post>> GetAllAsync(GetAllPostFilter filter);
        Task<List<GetAllPostResponse>> GetOrderedPostsAsync(GetAllPostOrderByPointWithPaging paging);
        Task<List<GetAllPostResponse>> GetPostVideosAsync(GetPostVideosOrderByCreatedWithPaging paging);
        Task<List<GetAllPostResponse>> GetPostsByUserIdAsync(GetPostByUserIdQuery request);
        Task<int> UpdateCommentCount(string Id);
        Task<long> GetTotalAsync(GetAllPostFilter filter);
        void InsertPosts(List<Post> posts);
        Task<int> CountPosts();

    }
}