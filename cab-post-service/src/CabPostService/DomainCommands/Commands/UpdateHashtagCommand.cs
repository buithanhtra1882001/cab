using MediatR;

namespace CabPostService.DomainCommands.Commands
{
    public class UpdateHashtagCommand : INotification
    {
        public string Hashtags {  get; set; }   
    }
}
