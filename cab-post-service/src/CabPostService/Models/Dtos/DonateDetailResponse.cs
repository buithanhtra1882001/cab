using System.Net;

namespace CabPostService.Models.Dtos
{
    public class DonateDetailResponse
    {
        public Guid Id { get; set; }

        public string Donater { get; set; }

        public string Receiver { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public long Coin { get; set; }

        public DateTime CreateDonate { get; set; }

        public HttpStatusCode Status { get; set; }
    }
}
