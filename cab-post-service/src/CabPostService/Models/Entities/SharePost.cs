using CabPostService.Models.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace CabPostService.Models.Entities
{
    public class SharePost : BaseEntity
    {
        public Guid Id { get; set; }
        public string PostId { get; set; }
        public Guid UserId { get; set; }
        public Guid? SharedUserId { get; set; }
        [ForeignKey("SharedUserId")]
        public virtual PostMiniUser ShareUser { get; set; }
        public string ShareLink { get; set; }
        public bool IsPublic { get; set; }
    }
}
