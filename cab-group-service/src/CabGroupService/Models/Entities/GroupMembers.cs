using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CabGroupService.Constants;
using Microsoft.EntityFrameworkCore;
using CabGroupService.Models.Entities.Base;

namespace CabGroupService.Models.Entities
{
    [Index(nameof(GroupID), nameof(UserID), IsUnique = true)]
    public class GroupMembers: BaseEntity
    {
        [Key]
        [Column(Order = 0)]
        [Required]
        public Guid GroupID { get; set; }
        [ForeignKey("GroupID")]
        public Group Group { get; set; }
        [Key]
        [Column(Order = 1)]
        [Required]
        public Guid UserID { get; set; }
        public DateTime JoinDate { get; set; }
        public JoinMethod JoinMethod { get; set; }
        public DateTime LastActiveDate { get; set; }
        public Decimal ContributionScore { get; set; }
        public int WarningsCount { get; set; } = 0;
        public Guid InvitedBy { get; set; }
        public bool NotificationSettings { get; set; } = true;
        public int ReputationPoints { get; set; } = 0;
        public string? JoinReason { get; set; }
        public string? Note { get; set; }
        public GroupMemberStatus Status { get; set; }
        public GroupPermissions Permissions { get; set; }
        public Guid DecentralizationUser { get; set; }
        public Guid UserApproval { get; set; }
    }
}
