using System.ComponentModel;
using System.Text.RegularExpressions;

namespace CabPostService.Constants
{

    public enum AppError
    {
        [Description("Unknow")]
        UNKNOWN,
        [Description("Operation fail")]
        OPERATION_FAIL,
        [Description("Unauthorized")]
        UNAUTHORIZED,
        [Description("Invalid parameters")]
        INVALID_PARAMETERS,
        [Description("Invalid operation")]
        INVALID_OPERATION,
        [Description("Permission denied")]
        PERMISSION_DENIED,
        [Description("System so busy")]
        SYSTEM_BUSY,
        [Description("Too many requests")]
        TOO_MANY_REQUESTS,
        [Description("Not found")]
        RECORD_NOTFOUND,
        [Description("Token invalid")]
        TOKEN_INVALID,
        [Description("Token wrong")]
        TOKEN_WRONG,
        [Description("Token expired")]
        TOKEN_EXPIRED,
        [Description("OTP invalid")]
        OTP_INVALID,
        [Description("OTP wrong")]
        OTP_WRONG,
        [Description("OTP expired")]
        OTP_EXPIRED,
        [Description("Password wrong")]
        PASSWORD_WRONG,
        [Description("Google authenticator code is required")]
        GACODE_REQUIRED,
        [Description("Google authenticator code wrong")]
        GACODE_WRONG,
        [Description("Credentials no match")]
        WRONG_CREDENTIALS,
        [Description("User is inactive")]
        USER_INACTIVE,
        [Description("User is baned")]
        USER_BANED,
        [Description("Insufficient funds")]
        INSUFFICIENT_FUNDS,
        [Description("Email exist")]
        EMAIL_EXIST,
        [Description("Sold out")]
        SOLD_OUT,
        [Description("Owner error")]
        OWNER_ERROR,
        [Description("Google authenticator error")]
        GOOGLE_LOGIN_FAIL
    }
    public enum ReportReason
    {
        NEGATIVE_CONTENT = 0,    // Nội dung tiêu cực
        FORBIDDEN_CONTENT = 1,   // Nội dung bị cấm
        SPAM = 2,
        COPYRIGHT_VIOLATION = 3, // Vi phạm bản quyền
        FAKE_NEWS = 4,           // Tin giả
        OTHER = 6,
    }

    public enum UserActionType
    {
        Like,
        Comment,
        Reply
    }
    public enum LikeType
    {
        Like = 1,
        Unlike = -1
    }

    public enum GroupPostStatus
    {
        [Description("Pending Approval")]
        PENDING_APPROVAL,
        [Description("Approved")]
        APPROVED,
        [Description("Do Not Approve")]
        DO_NOT_APPROVE,
    }
}
