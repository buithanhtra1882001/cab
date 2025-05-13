using Autofac.Extensions.DependencyInjection;
using CAB.BuildingBlocks.EventBusRegistration;
using CabPaymentService.Infrastructures.DbContexts;
using CabPaymentService.Infrastructures.Startup.PipelineExtensions;
using CabPaymentService.Infrastructures.Startup.ServicesExtensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using System.Net;
using WebHost.Customization;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = CabPaymentService.Infrastructures.Loggings.LoggerFactory.CreateLogger(builder.Configuration);

// Add services to the container.
builder.Services.AddGeneralConfigurations(builder.Configuration);
builder.Services.AddSwaggerService();
builder.Services.AddInjectedServices(builder.Configuration);
builder.Services.AddGrpc();
builder.Services.AddUploadServices();
builder.Services.AddMinioService(builder.Configuration);
builder.Services.AddEventBus(builder.Configuration);

builder.Host
    .UseSerilog(CabPaymentService.Infrastructures.Loggings.LoggerFactory.SetupLogger)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.WebHost
    .UseUrls()
    .UseKestrel()
    .ConfigureKestrel(options =>
    {
        var grpcPort = builder.Configuration.GetValue("GRPC_PORT", 10004);
        var httpPort = builder.Configuration.GetValue("PORT", 9004);
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

try
{
    app.MigrateDbContext<PostgresDbContext>((_, __) => { }).Run();
    //app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
