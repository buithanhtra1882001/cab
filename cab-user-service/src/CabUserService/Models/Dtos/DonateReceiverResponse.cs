using System.Net;

namespace CabUserService.Models.Dtos
{
    public class DonateReceiverResponse
    {
        public string Donater { get; set; }

        public string Receiver { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public long Coin { get; set; }

        public HttpStatusCode Status { get; set; }
    }
}
