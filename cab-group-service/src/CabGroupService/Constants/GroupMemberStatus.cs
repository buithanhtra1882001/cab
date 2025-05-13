using System.ComponentModel;

namespace CabGroupService.Constants
{
    public enum GroupMemberStatus
    {
        [Description("Active")]
        ACTIVE,
        [Description("Banned")]
        BANNED,
        [Description("Pending Confirmation")]
        PENDING,
    }
}
