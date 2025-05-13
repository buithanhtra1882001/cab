using CabPostService.EventHub;
using CabPostService.EventHub.SignalRHub;
using CabPostService.Grpc.Procedures;
using CabPostService.Infrastructures.Middlewares;

namespace CabPostService.Infrastructures.Startup.PipelineExtensions
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

            // Middleware để bỏ qua xác thực cho SignalR
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/hubs/post/donate"))
                    await next();                
                else
                {
                    app.UseAuthentication();
                    app.UseAuthorization();
                    await next();
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<PostService>();
                // Config Hub
                endpoints.MapHub<DonateHub>("/hubs/post/donate");
            });
        }
    }
}