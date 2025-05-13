using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Commands
{
    public class LikeOrUnlikeCommentCommand : ICommand<bool>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public Guid CommentId { get; set; }
        public LikeType Type { get; set; }
    }
}
