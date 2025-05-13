using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using MediatR;

namespace CabPostService.Cqrs.Requests.Queries
{
    public class GetDetailDonateQuery : IQuery<List<DonateDetailResponse>>
    {
    }
}