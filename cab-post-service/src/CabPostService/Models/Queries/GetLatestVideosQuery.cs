using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using Newtonsoft.Json;

namespace CabPostService.Models.Queries
{
    public class GetLatestVideosQuery : IQuery<PagingResponse<GetAllPostResponse>?>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
