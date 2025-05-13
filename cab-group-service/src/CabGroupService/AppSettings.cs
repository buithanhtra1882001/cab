namespace CabGroupService
{
    public class AuthenticationSetting
    {
        public string JwtSigninKey { get; set; }
    }

    public class AppSettings
    {
        public AuthenticationSetting Authentication { get; set; }
    }
}