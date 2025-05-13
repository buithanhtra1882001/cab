using System.Net;
using Autofac.Extensions.DependencyInjection;
using CAB.BuildingBlocks.EventBusRegistration;
using CabMediaService.Infrastructures.DbContexts;
using CabMediaService.Infrastructures.Startup.PipelineExtensions;
using CabMediaService.Infrastructures.Startup.ServicesExtensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = CabMediaService.Infrastructures.Loggings.LoggerFactory.CreateLogger(builder.Configuration);

builder.Services.AddGeneralConfigurations(builder.Configuration);
builder.Services.AddLazyCache();
builder.Services.AddGrpc();
builder.Services.AddSwaggerService();
builder.Services.AddInjectedServices(builder.Configuration);
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddAWSService(builder.Configuration);
builder.Services.AddDropboxService(builder.Configuration);
builder.Services.AddUploadServices();

builder.Host
    .UseSerilog(CabMediaService.Infrastructures.Loggings.LoggerFactory.SetupLogger)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.WebHost
    .UseUrls()
    .UseKestrel()
    .ConfigureKestrel(options =>
    {
        var grpcPort = builder.Configuration.GetValue("GRPC_PORT", 10005);
        var httpPort = builder.Configuration.GetValue("PORT", 9005);
        options.Listen(IPAddress.Any, httpPort, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        });
        options.Listen(IPAddress.Any, grpcPort, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http2;
        });
    })
    .UseContentRoot(Directory.GetCurrentDirectory());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseGeneralConfigurations(app.Environment);
app.UseSwaggerExposer(app.Environment);
app.UseEventBusSubcribers();
app.Run();

try
{
    app.MigrateDbContext<PostgresDbContext>((_, __) => { }).Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}