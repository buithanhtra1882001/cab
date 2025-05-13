using CabPostService.Models.Entities.Base;

namespace CabPostService.Models.Entities
{
    public class PostDonate : BaseEntity
    {
        public Guid Id { get; set; }
        public string PostId { get; set; }
        public Guid DonaterId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Value { get; set; }
    }
}
