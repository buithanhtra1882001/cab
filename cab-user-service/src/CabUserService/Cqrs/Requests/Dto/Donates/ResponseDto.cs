using System.Net;

namespace CabUserService.Cqrs.Requests.Dto.Donates
{
    public class ResponseDto
    {
        public HttpStatusCode Status { get; set; }

        public string Message { get; set; }
    }
}
