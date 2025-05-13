using CabUserService.Hubs;
using CabUserService.Infrastructures.Middlewares;

namespace CabUserService.Infrastructures.Startup.PipelineExtensions
{
    public static class GeneralPipelineExtension
    {
        public static void UseGeneralConfigurations(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseHangfireDashboard();
            }

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHealthChecks("/healthcheck");
            //app.ConfigureHangfire();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/hubs/chat");
                endpoints.MapHub<PresentHub>("/hubs/present");
                endpoints.MapHub<LiveDonationNotificationHub>("/hubs/liveDonation");
                //endpoints.MapHangfireDashboard();
            });
        }
    }
}