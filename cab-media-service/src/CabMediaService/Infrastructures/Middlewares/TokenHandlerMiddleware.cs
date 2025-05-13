using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace CabMediaService.Infrastructures.Middlewares
{
    public class TokenHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _excludePaths = new string[] {
            "/swagger/",
        };

        public TokenHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // If request path is not in exclude list
            if (!_excludePaths.Any(p => context.Request.Path.Value.StartsWith(p))) {
                if (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var authStringValues)) {
                    var bearerToken = authStringValues.FirstOrDefault().Replace("Bearer ", "");
                    var jwtToken = new JwtSecurityToken(bearerToken);
                    var uuid = jwtToken.Claims.FirstOrDefault(x => x.Type.ToLowerInvariant() == "uuid").Value;
                    context.Request.QueryString = context.Request.QueryString.Add(nameof(uuid), uuid);
            }
                else {
                    await WriteUnauthorized(context);
                }
            }

            await _next(context);
        }

        public async Task WriteUnauthorized(HttpContext context)
        {
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
        }
    }
}