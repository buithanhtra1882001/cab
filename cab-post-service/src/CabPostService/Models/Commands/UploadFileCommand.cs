using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;

namespace CabPostService.Models.Commands
{
    public class UploadFileCommand : ICommand<BaseResponse>
    {
        public Guid UserId { get; set; }
        public IFormFileCollection Files { get; set; }
    }
}
