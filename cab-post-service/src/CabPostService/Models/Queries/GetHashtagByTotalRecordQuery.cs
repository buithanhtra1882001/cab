using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Queries
{
    public class GetHashtagByTotalRecordQuery : IQuery<IList<HashtagResponse>>
    {
        public int TotalRecord { get; set; }
        public string Keyword { get; set; }
        public string Type { get; set; }
    }
}
