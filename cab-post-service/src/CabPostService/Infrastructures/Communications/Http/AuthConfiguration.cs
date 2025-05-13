
using CabPostService.Infrastructures.Extensions;

namespace CabPostService.Infrastructures.Communications.Http
{
    public class AuthConfiguration
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string BearerToken { get; set; }
        public AuthSchemes Scheme { get; set; }

        public AuthConfiguration()
        {
            Scheme = AuthSchemes.None;
        }

        public AuthConfiguration(string bearerToken)
        {
            BearerToken = bearerToken;
            Scheme = AuthSchemes.BearerToken;
        }

        public AuthConfiguration(string username, string password)
        {
            Username = username;
            Password = password;
            Scheme = AuthSchemes.Basic;
        }

        public void Validate()
        {
            if (Scheme == AuthSchemes.Basic)
            {
                if (Username.IsNullOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(Username));
                }

                if (Password.IsNullOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(Password));
                }
            }
            else if (Scheme == AuthSchemes.BearerToken)
            {
                if (BearerToken.IsNullOrWhiteSpace())
                {
                    throw new ArgumentNullException(nameof(BearerToken));
                }
            }
            else
            {
                return;
            }
        }
    }
}