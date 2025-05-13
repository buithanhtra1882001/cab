using System.Collections.Generic;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Token
{
    public interface IJwtTokenGenerator
    {
        public JwtTokenModel Generate(IDictionary<string, object> claims);
    }
}