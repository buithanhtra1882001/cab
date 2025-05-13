using CabPostService.Handlers.Interfaces;
using System.ComponentModel.DataAnnotations;
using CabPostService.Models.Dtos;

namespace CabPostService.Models.Commands
{
    public class UpdateAVTPCommand : ICommand<BaseResponse>
    {
        [Required]
        public string PostVideoId { get; set; }

        [Required]
        public double TimeVideoView { get; set; }
    }
}
