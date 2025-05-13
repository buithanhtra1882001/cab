using CabPostService.Handlers.Interfaces;

namespace CabPostService.Models.Commands
{
    public class CreatePostVideoCommand : ICommand<string>
    {
        public Guid UserId { get; set; }
        public string MediaVideoId { get; set; }
        public string VideoUrl { get; set; }
        public string Description { get; set; }
        public double LengthVideo { get; set; }
        public double AvgViewLength { get; set; }
        public bool IsViolence { get; set; }
    }
}
