using Serilog;

namespace CabUserService.Infrastructures.Loggings
{
    public static class LoggerFactory
    {
        public static WebApplicationBuilder AddSerilog(
            this WebApplicationBuilder builder,
            IConfiguration configuration)
        {
            var pathLogFile = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log-donatation-.txt");

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Logger(writeTo => writeTo.Filter
                                .ByIncludingOnly(e => e.Properties.ContainsKey("donatation"))
                                .WriteTo.File(pathLogFile, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true))
                .WriteTo.Async(writeTo => writeTo.Console())
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Host.UseSerilog(Log.Logger, true);

            return builder;
        }

        public static WebApplication UseSerilog(this WebApplication app)
        {
            return app;
        }
    }
}