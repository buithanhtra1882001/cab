using CabPostService.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CabPostService.Models.Entities
{
    public class PostVote : BaseEntity
    {
        [Key, Column(Order = 0)]
        public Guid Id { get; set; }

        [Key, Column(Order = 1)]
        public string PostId { get; set; }

        [Key, Column(Order = 2)]
        public Guid UserVoteId { get; set; }

        public virtual Post Post { get; set; }

        public int Type { get; set; }
    }
}
