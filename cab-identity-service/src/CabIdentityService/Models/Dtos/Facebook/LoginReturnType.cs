using WCABNetwork.Cab.IdentityService.Infrastructures.Token;

namespace WCABNetwork.Cab.IdentityService.Models.Dtos
{
    public class LoginReturnType 
    {
        public JwtTokenModel token;
        public bool accountLockedOut;
        public bool incorrectCredential;
    }
}
