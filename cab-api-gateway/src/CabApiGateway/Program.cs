using CabApiGateway.Infrastructures.OcelotConfigurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using System.IO;

namespace CabApiGateway
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Logger = SetStaticLogger();
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog(SetupLogger)
                .ConfigureAppConfiguration(OcelotRouteConfiguration.Setup)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                                .UseContentRoot(Directory.GetCurrentDirectory());
                });
        }

        private static void SetupLogger(HostBuilderContext hostingContext, LoggerConfiguration loggerConfiguration)
        {
            loggerConfiguration
                .Destructure
                .ToMaximumDepth(5)
                .ReadFrom
                .Configuration(hostingContext.Configuration);
        }

        private static ILogger SetStaticLogger()
        {
            return new LoggerConfiguration()
                .WriteTo
                .Console()
                .CreateLogger();
        }
    }
}