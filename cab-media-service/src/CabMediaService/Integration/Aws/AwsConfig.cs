namespace CabMediaService.Integration.Configs
{
    public class AwsConfig
    {
        public S3Config S3 { get; set; }
        public SESConfig SES { get; set; }
    }

    public class S3Config
    {
        public string? AccessKey { get; set; }
        public string? SecretKey { get; set; }
        public string? Region { get; set; }
        public string? Bucket { get; set; }
    }
    public class SESConfig
    {
        public string? MailFrom { get; set; }
        public string? MailName { get; set; }
        public string? SMTPUsername { get; set; }
        public string? SMTPPassword { get; set; }
        public int Port { get; set; }
        public string? Host { get; set; }
    }
}
