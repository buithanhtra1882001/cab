using CabMediaService.Grpc.Procedures;
using CabMediaService.Infrastructures.Middlewares;

namespace CabMediaService.Infrastructures.Startup.PipelineExtensions
{
    public static class GeneralPipelineExtension
    {
        public static void UseGeneralConfigurations(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<TokenHandlerMiddleware>();
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseHealthChecks("/healthcheck");

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<MediaService>();
            });
        }
    }
}