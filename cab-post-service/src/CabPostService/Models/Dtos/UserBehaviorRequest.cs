using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;

namespace CabPostService.Models.Dtos
{
    public class UserBehaviorRequest : ICommand<Guid>
    {
        public Guid UserId { get; set; }
        public string PostId { get; set; }
        public UserActionType Type { get; set; }
    }
}
