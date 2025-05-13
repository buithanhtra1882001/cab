using CabPostService.Constants;

namespace CabPostService.Models.Dtos
{
    public class PostInteractionResponse
    {
        public UserActionType Type { get; set; }
        public UserPostResponse UserPostResponse { get; set; }
    }
}
