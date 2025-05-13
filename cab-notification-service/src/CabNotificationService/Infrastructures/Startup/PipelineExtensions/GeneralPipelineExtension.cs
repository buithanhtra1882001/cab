using CabNotificationService.Hubs;
using CabNotificationService.Infrastructures.Middlewares;

namespace CabNotificationService.Infrastructures.Startup.PipelineExtensions
{
    public static class GeneralPipelineExtension
    {
        public static void UseGeneralConfigurations(
            this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            //app.UseMiddleware<TokenHandlerMiddleWare>();
            app.UseHealthChecks("/healthcheck");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/hub/notification");
            });
        }
    }
}