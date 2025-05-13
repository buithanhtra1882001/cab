using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using MediatR;

namespace CabPostService.Cqrs.Requests.Queries
{
    public class GetLstReceiveAmountsByIdQuery : IQuery<List<GetLstReceiveAmountsByIdResponse>>
    {
        public Guid UserId { get; set; }
    }
}