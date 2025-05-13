using CabUserService.Models.Dtos;
using MailKit;
using MediatR;

namespace CabUserService.Cqrs.Requests.Queries
{
    public class StatisticalUserQuery : IRequest<StatisticalUserDto>
    {
        public Guid UserId { get; set; }

        public DateTime? FromDate { get; set; }
        
        public DateTime? ToDate { get; set; }

        public string DonaterId { get; set; }

        public string ReactionId { get; set; }
    }
}