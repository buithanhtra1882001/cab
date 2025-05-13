using CabPostService.Models.Dtos;
using MediatR;

namespace CabPostService.Models.Queries
{
    public class PopulatePostDetailsQuery : IRequest<List<GetAllPostResponse>>
    {
        public List<GetAllPostResponse> Posts { get; set; }
        public Guid UserId { get; set; }
    }
}
