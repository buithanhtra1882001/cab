using System.ComponentModel;

namespace CabUserService.Constants
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

    public enum TransactionStatus
    {
        PENDING = 0,
        SUCCESS = 1,
        FAIL = 2,
        CANCEL = 3,
    }

    public enum TransactionType
    {
        TRANSFER = 0,
        WITHDRAW = 1,
        DEPOSIT = 2
    }

    public enum BalanceType
    {
        DONATAION = 0
    }
    public enum IntervalType
    {
        WEEK = 0,
        MONTH = 1,
        YEAR = 2
    }

    public enum UserType : byte
    {
        NORMAL = 0,
        CONTENT_CREATOR = 1,
    }

    public enum FOLLOW_TYPE
    {
        FOLLOWER = 1,
        FOLLOWING = 2,
    }

    public enum ACTION_FRIEND_TYPE
    {
        CANCEL = 0,
        SEND_REQUEST = 1,
    }
    public enum REQUEST_TYPE
    {
        FRIEND = 0,
        CREATOR = 1,
    }
    public enum ACCEPTANCE_STATUS
    {
        NORMAL = 0,
        ACCEPTED = 1,
        NOTACCEPTED = 2,
    }

    public enum DonateType
    {
        None = 0,
        NormalDonate = 1,
        PostDonate = 2,
    }
}
