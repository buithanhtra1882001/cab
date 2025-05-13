using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CabGroupService.Constants;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using CabGroupService.Models.Entities.Base;

namespace CabGroupService.Models.Entities
{
    public class Group : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        [Unicode(true)]
        public string? GroupName { get; set; }
        [Unicode(true)]
        public string? GroupDescription { get; set; }
        public GroupType GroupType { get; set; }
        public string? GroupTagline { get; set; }
        public string? CoverPhoto { get; set; }
        public string? ProfilePicture { get; set; }
        public Guid CreatedByUser { get; set; }
        public DateTime? LastActivityDate { get; set; } = DateTime.UtcNow;
        public int MemberCount { get; set; } = 0;
        public string? PrivacySettings { get; set; }
        public string? Rules { get; set; }
        public string? Location { get; set; }
        public string? WebsiteURL { get; set; }
        public string? ContactEmail { get; set; }
        public string? TagList { get; set; }
        public bool ApprovalRequired { get; set; } = false;
        public int JoinRequestCount { get; set; } = 0;
    }
}
