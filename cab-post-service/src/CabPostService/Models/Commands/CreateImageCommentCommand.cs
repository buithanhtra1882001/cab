using CabPostService.Handlers.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Commands
{
    public class CreateImageCommentCommand:  ICommand<Guid>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }

        [Required]
        public Guid ImageId { get; set; }
        public string Content { get; set; }
    }
}
