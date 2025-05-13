using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace CabMediaService.Controllers.Base
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class ApiController<T> : ControllerBase
    {
        protected readonly ILogger<T> _logger;
        protected string BearerToken
        {
            get
            {
                return Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            }
        }
        public ApiController(ILogger<T> logger)
        {
            _logger = logger;
        }

        protected Guid GetUserIdFromToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(BearerToken) as JwtSecurityToken;

            if (jwtToken == null)
                throw new SecurityTokenException("Invalid token");

            var userIdClaim = jwtToken.Claims.First(claim => claim.Type == "Uuid").Value;
            if (Guid.TryParse(userIdClaim, out Guid userId))
                return userId;

            throw new Exception("Invalid userId format");
        }
    }
}