using CabGroupService.Constants;

namespace CabGroupService.Models.Dtos
{
    public class GroupMemberPendingApproval
    {
        public Guid GroupID { get; set; }
        public Guid UserId { get; set; }
        public Guid InvitedBy { get; set; }
        public string? JoinReason { get; set; }
        public GroupMemberStatus Status { get; set; }
    }
}
