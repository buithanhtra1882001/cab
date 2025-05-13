using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Queries;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CabPostService.Handlers.CurrentUser
{
    public class UserHttpContextQueryHandler :
        IQueryHandler<UserHttpContextQuery, Guid>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AppSettings _appSettings;
        public UserHttpContextQueryHandler(
            IServiceProvider serviceProvider,
            IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
            _serviceProvider = serviceProvider;
        }

        public async Task<Guid> Handle(
            UserHttpContextQuery request,
            CancellationToken cancellationToken)
        {
            var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();

            if (httpContextAccessor is null || httpContextAccessor.HttpContext is null)
                return new Guid();

            var claims = ParseAuthenticationHeader(httpContextAccessor.HttpContext);
            if (claims == null || !claims.Any())
                return new Guid();

            _ = Guid.TryParse(claims.FirstOrDefault(x => x.Type == "Uuid")?.Value, out Guid userId);

            return await Task.FromResult(userId);
        }

        private IEnumerable<Claim> ParseAuthenticationHeader(HttpContext httpContext)
        {
            var claims = httpContext.User.Claims.ToList();

            if (claims.Any()) return claims;

            if (httpContext.Request == null ||
                httpContext.Request.Headers == null ||
                !httpContext.Request.Headers.ContainsKey("Authorization"))
                return new List<Claim>();

            var token = httpContext.Request
                .Headers["Authorization"]
                .FirstOrDefault()?
                .Split(" ").Last();

            if (string.IsNullOrEmpty(token))
                return new List<Claim>();

            var result = ParseToken(token, BuildKey());
            if (result == null) return new List<Claim>();

            return result.Claims;
        }

        private JwtSecurityToken? ParseToken(
            string token,
            SymmetricSecurityKey key)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken;
            }
            catch
            {
            }
            return null;
        }

        private byte[] GetConfigKeyAsBytes()
        {
            var issuerSigningKey = _appSettings.Authentication.JwtSigninKey;
            var data = Encoding.UTF8.GetBytes(issuerSigningKey);
            return data;
        }

        private SymmetricSecurityKey BuildKey()
        {
            var data = GetConfigKeyAsBytes();
            var result = new SymmetricSecurityKey(data);
            return result;
        }
    }
}
