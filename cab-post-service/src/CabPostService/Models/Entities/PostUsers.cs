using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CabPostService.Models.Entities
{
    public class PostUsers
    {
        [Key, Column(Order = 0)]
        public string PostId { get; set; }

        [Key, Column(Order = 1)]
        public Guid UserId { get; set; }

        public bool IsShowPost { get; set; }

        [ForeignKey("UserId")]
        public virtual PostMiniUser User { get; set; }

        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }
    }
}
