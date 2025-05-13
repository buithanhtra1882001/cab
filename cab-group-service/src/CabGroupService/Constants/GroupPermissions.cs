using System.ComponentModel;

namespace CabGroupService.Constants
{
    public enum GroupPermissions
    {
        [Description("Admin")]
        ADMIN,
        [Description("Moderator")]
        MODERATOR,
        [Description("Member")]
        MEMBER
    }
}
