using CabGroupService.Constants;

namespace CabGroupService.Models.Dtos.Group
{
    public class GroupResponse
    {
        public Guid Id { get; set; }
        public string? GroupName { get; set; }
        public string? GroupDescription { get; set; }
        public GroupType GroupType { get; set; }
        public string? GroupTagline { get; set; }
        public string? CoverPhoto { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public int MemberCount { get; set; }
        public string? PrivacySettings { get; set; }
        public string? Rules { get; set; }
        public string? Location { get; set; }
        public string? WebsiteURL { get; set; }
        public string? ContactEmail { get; set; }
        public string? TagList { get; set; }
        public bool ApprovalRequired { get; set; } = false;
        public Guid CreatedByUser { get; set; }
    }
}
