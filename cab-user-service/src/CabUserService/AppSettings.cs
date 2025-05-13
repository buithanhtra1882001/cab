namespace CabUserService
{
    public class STMPSetting
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
    }

    public class AppSettings
    {
        public STMPSetting STMPSetting { get; set; }
    }
}