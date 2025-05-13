using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace CabApiGateway.Infrastructures.Token
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JwtTokenModel Generate(IDictionary<string, object> claims)
        {
            var expireTime = GetExpireTime(_configuration);
            var key = JwtSymmetricSecurityBuilder.BuildKey(_configuration);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = expireTime,
                SigningCredentials = credentials,
                Claims = claims
            };

            var jwtHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtHandler.CreateToken(tokenDescriptor);
            var newToken = jwtHandler.WriteToken(securityToken);
            var expirySpan = expireTime - DateTime.UtcNow;

            return new JwtTokenModel
            {
                AccessToken = newToken,
                ExpiresIn = (int)expirySpan.TotalSeconds
            };
        }

        private DateTime GetExpireTime(IConfiguration configuration)
        {
            var expiredTimeInSecond = configuration.GetValue<int>("Authentication:AccessTokenExpiredTimeInSecond");
            return DateTime.UtcNow.AddSeconds(expiredTimeInSecond);
        }
    }
}