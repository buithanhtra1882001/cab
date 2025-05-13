using System.Net;

namespace CabPostService.Models.Dtos
{
    public class ResponseDto
    {
        public HttpStatusCode Status { get; set; }

        public string Message { get; set; }
    }
}
