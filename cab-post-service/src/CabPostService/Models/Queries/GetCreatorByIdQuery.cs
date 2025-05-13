using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;

namespace CabPostService.Models.Queries
{
    public class GetCreatorByIdQuery : IQuery<IList<CreatorResponse>>
    {
        public Guid UserId { get; set; }
        public List<Guid>? CreatorIds { get; set; }
    }
}
