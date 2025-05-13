using CabPostService.Handlers.Interfaces;
using Newtonsoft.Json;

namespace CabPostService.Models.Commands
{
    public class VoteDownPostCommand : ICommand<bool>
    {
        public string PostId { get; set; }
        [JsonIgnore]
        public Guid UserId { get; set; }

    }
}
