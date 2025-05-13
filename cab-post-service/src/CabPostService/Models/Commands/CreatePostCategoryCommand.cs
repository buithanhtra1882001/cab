using CabPostService.Handlers.Interfaces;

namespace CabPostService.Models.Commands
{
    public class CreatePostCategoryCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public int Status { get; set; }
        public bool IsSoftDeleted { get; set; }
    }
}
