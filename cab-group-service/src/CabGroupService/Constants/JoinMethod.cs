using System.ComponentModel;

namespace CabGroupService.Constants
{
    public enum JoinMethod
    {
        [Description("Invited")]
        Invited,
        [Description("Requested")]
        Requested,
        [Description("Auto-joined")]
        AutoJoined,
    }
}
