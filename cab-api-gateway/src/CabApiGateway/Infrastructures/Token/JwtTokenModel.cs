namespace CabApiGateway.Infrastructures.Token
{
    public class JwtTokenModel
    {
        public string TokenType { get; set; } = "Bearer";
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }
}