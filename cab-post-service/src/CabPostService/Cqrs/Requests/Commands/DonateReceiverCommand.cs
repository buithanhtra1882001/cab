using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using MediatR;

namespace CabPostService.Cqrs.Requests.Commands
{
    public class DonateReceiverCommand : ICommand<ResponseDto>
    {
        public string PostId { get; set; }

        public Guid ReceiverId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int Coin { get; set; }

        public string? Token { get; set; }
    }
}