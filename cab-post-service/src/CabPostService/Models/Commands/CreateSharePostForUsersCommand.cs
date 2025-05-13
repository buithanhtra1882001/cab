using CabPostService.Handlers.Interfaces;

namespace CabPostService.Models.Commands
{
    public class CreateSharePostForUsersCommand : ICommand<IList<Guid>>
    {
        public Guid UserId { get; set; }
        public string PostId { get; set; }
        public List<Guid> SharedUserIds { get; set; }
        public string ShareLink { get; set; }
        public bool IsPublic { get; set; }
    }
}
