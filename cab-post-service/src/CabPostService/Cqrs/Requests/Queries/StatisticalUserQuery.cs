using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using MediatR;

namespace CabPostService.Cqrs.Requests.Queries
{
    public class StatisticalUserQuery : IQuery<StatisticalUserDto>
    {
        public Guid UserId { get; set; }

        public DateTime? FromDate { get; set; }
        
        public DateTime? ToDate { get; set; }
    }
}