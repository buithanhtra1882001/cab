using AspNetCoreRateLimit;
using Autofac.Extensions.DependencyInjection;
using CabIdentityService.Infrastructures.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRegistration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using WCABNetwork.Cab.IdentityService.Infrastructures.DbContexts;
using WCABNetwork.Cab.IdentityService.Infrastructures.Middlewares;
using WCABNetwork.Cab.IdentityService.Infrastructures.Startup.DataSeeder;
using WCABNetwork.Cab.IdentityService.Infrastructures.Startup.PipelineExtensions;
using WCABNetwork.Cab.IdentityService.Infrastructures.Startup.ServicesExtensions;

try
{
    Log.Logger = SetStaticLogger();
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;
    var env = builder.Environment;

    builder.Host.UseSerilog(SetupLogger).UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Services.AddGeneralConfigurations(configuration);
    builder.Services.AddSwaggerService();
    builder.Services.AddIdentityCore();
    builder.Services.AddInjectedServices(configuration);
    builder.Services.AddEventBus(configuration);

    if (!env.IsDevelopment())
    {
        builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        builder.Services.Configure<RateLimitOptions>(configuration.GetSection("IpRateLimiting"));
    }

    //builder.Services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

    builder.Services.AddScoped<ExceptionHandlingMiddleware>();

    builder.Services.AddOptions();
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
    builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    builder.Services.AddInMemoryRateLimiting();

    var app = builder.Build();

    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // fix postgres datetime Kind=UTC bug in net 6

    //app.UseIpRateLimiting();
    app.UseMiddleware<CustomRateLimitMiddleWare>();
    app.UseMiddleware<ExceptionHandlerMiddleware>();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.MigrateDbContext<IdentityCoreDbContext>((_, serviceProvider) => SeedData(serviceProvider));
    app.UseGeneralConfigurations(env);
    app.UseSwaggerExposer(env);
    app.UseEventBusSubcribers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static ILogger SetStaticLogger()
{
    return new LoggerConfiguration().WriteTo.Console()
                                            .CreateLogger();
}

static void SetupLogger(HostBuilderContext hostingContext, LoggerConfiguration loggerConfiguration)
{
    var configuration = hostingContext.Configuration.GetSection("Serilog");

    if (bool.TrueString.Equals(configuration["RenderJson"], StringComparison.OrdinalIgnoreCase))
    {
        loggerConfiguration.WriteTo.Console(new RenderedCompactJsonFormatter());
    }
    else
    {
        loggerConfiguration.WriteTo.Console(outputTemplate: configuration["ConsoleTemplate"],
            restrictedToMinimumLevel: Enum.Parse<LogEventLevel>(configuration["MinimumLevel"]));
    }

    loggerConfiguration.Enrich.FromLogContext();
}

static void SeedData(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetService<RoleManager<IdentityRole<int>>>();
    DataInitializer.SeedData(roleManager);
}
