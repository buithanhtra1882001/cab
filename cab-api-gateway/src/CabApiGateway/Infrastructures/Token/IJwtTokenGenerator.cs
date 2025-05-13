using System.Collections.Generic;

namespace CabApiGateway.Infrastructures.Token
{
    public interface IJwtTokenGenerator
    {
        public JwtTokenModel Generate(IDictionary<string, object> claims);
    }
}