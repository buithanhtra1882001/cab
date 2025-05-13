using CabPostService.Models.Entities;

namespace CabPostService.Models.Dtos
{
    public class PostCommentPostIdMaterializedViewResponse
    {
        public List<PostCommentPostIdMaterializedView> PostCommentPostIdMaterializedViews { get; set; }
        public string PagingState { get; set; }
    }
}
