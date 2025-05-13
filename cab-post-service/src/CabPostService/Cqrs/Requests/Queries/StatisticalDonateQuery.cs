using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using MediatR;

namespace CabPostService.Cqrs.Requests.Queries
{
    public class StatisticalDonateQuery : IQuery<StatisticalDonateDto>
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public Guid? UserId { get; set; }
    }
}