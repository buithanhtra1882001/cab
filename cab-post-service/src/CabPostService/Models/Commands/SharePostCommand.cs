using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;

namespace CabPostService.Models.Commands
{
    public class SharePostCommand : ICommand<SharePostResponse>
    {
        public Guid UserId { get; set; }
        public string PostId { get; set; }
        public string ShareLink { get; set; }
        public bool IsPublic { get; set; } = true;
    }
}
