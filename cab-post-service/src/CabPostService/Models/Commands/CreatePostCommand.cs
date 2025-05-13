using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;

namespace CabPostService.Models.Commands
{
    public class CreatePostCommand : ICommand<GetAllPostResponse>
    {
        public Guid UserId { get; set; }
        public string PostType { get; set; }
        public Guid CategoryId { get; set; }
        public List<ImageInfoRequest> ImageInfo { get; set; } = new List<ImageInfoRequest> { };
        public List<VideoInfoRequest> VideoInfo { get; set; } = new List<VideoInfoRequest> { };
        public string Hashtags { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public string? GroupId { get; set; }
        public bool IsPersonalPost { get; set; } = true;
        public GroupPostStatus IsApproved { get; set; } = GroupPostStatus.APPROVED;
    }

    public class ImageInfoRequest
    {
        public Guid ImageId { get; set; }

        public string ImageUrl { get; set; }
    }

    public class VideoInfoRequest
    {
        public string VideoId { get; set; }

        public string VideoUrl { get; set; }
    }
}
