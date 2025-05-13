using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;

namespace CabPostService.Models.Queries
{
    public class GetUserInteractedPostsQuery : IQuery<IList<PostInteractionResponse>>
    {
        public Guid UserId { get; set; }
    }
}
