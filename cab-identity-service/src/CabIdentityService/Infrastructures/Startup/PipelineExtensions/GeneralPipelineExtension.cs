using CabIdentityService.Infrastructures.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Startup.PipelineExtensions
{
    public static class GeneralPipelineExtension
    {
        public static void UseGeneralConfigurations(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            var supportedCultures = new[] { "vi" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            // This might crash the app
            // app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHealthChecks("/healthcheck");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}