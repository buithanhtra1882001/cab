using CabApiGateway.Infrastructures.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Ocelot.Authorization;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CabApiGateway.Infrastructures.Startup.PipelineExtensions
{
    public static class OcelotAuthorizeExtension
    {
        public static void UseAuthorizeConfigurations(this IApplicationBuilder app)
        {
            var ocelotConfiguration = new OcelotPipelineConfiguration
            {
                AuthorizationMiddleware = async (ctx, next) =>
                {
                    if (ctx.User.Identity.IsAuthenticated)
                    {
                        // todo: Check Cookie Security Hash
                        //CheckCookieSecurityHash(ctx);
                    }

                    await next.Invoke();

                }
            };
            app.UseOcelot(ocelotConfiguration).Wait();
        }

        private static void CheckCookieSecurityHash(HttpContext ctx)
        {
            try
            {
                var noPlaybackAttacked = false;
                
                //var cookieHash = ctx.Request.Cookies["fingerprint"]?.Sha256();
                var cookieHash = ctx.Request.Headers["Fingerprint"].FirstOrDefault();
                var hash = ctx.User.Claims.First(claim => claim.Type == "Fingerprint_Hash").Value;

                if (cookieHash == hash)
                {
                    noPlaybackAttacked = true;
                }

                if (!noPlaybackAttacked)
                {
                    ctx.Items.SetError(new UnauthorizedError($"Failed To Authorize. cookie fingerprint not match with JWT Anti_Jwt_Theft_Hash claim"));
                }

            }
            catch (Exception e)
            {
                ctx.Items.SetError(new UnauthorizedError(e.Message));
            }
        }
    }
}
