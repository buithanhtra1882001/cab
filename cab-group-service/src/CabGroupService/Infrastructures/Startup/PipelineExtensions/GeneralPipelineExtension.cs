using CabGroupService.Hubs;
using CabGroupService.Infrastructures.Middlewares;

namespace CabGroupService.Infrastructures.Startup.PipelineExtensions
{
    public static class GeneralPipelineExtension
    {
        public static void UseGeneralConfigurations( this IApplicationBuilder app, IWebHostEnvironment env){
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseHealthChecks("/healthcheck");

            app.UseRouting();
            app.UseCors("CabCors");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/hub/group");
            });
        }
    }
}