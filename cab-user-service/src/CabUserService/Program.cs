using Autofac.Extensions.DependencyInjection;
using CabPaymentService.Infrastructures.Startup.ServicesExtensions;
using CabUserService.Grpc.Procedures;
using CabUserService.Infrastructures.DbContexts;
using CabUserService.Infrastructures.Helper;
using CabUserService.Infrastructures.Loggings;
using CabUserService.Infrastructures.Middlewares;
using CabUserService.Infrastructures.Startup.PipelineExtensions;
using CabUserService.Infrastructures.Startup.ServicesExtensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGeneralConfigurations(builder.Configuration);
builder.Services.AddSwaggerService();
builder.Services.AddLazyCache();
builder.Services.AddInjectedServices(builder.Configuration);
builder.Services.AddGrpc();
builder.Services.AddSignalR();
builder.Services.AddUploadServices();
builder.Services.AddMinioService(builder.Configuration);
builder.Services.AddAndConfigMassTransit(builder.Configuration);
//builder.Services.AddHangfire(x => x.UsePostgreSqlStorage(builder.Configuration.GetValue<string>("UserDbConnectionString")));
//builder.Services.AddHangfireServer();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.WebHost
    .UseUrls()
    .UseKestrel()
    .ConfigureKestrel(options =>
    {
        var grpcPort = builder.Configuration.GetValue("GRPC_PORT", 10002);
        var httpPort = builder.Configuration.GetValue("PORT", 9002);
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

builder.AddSerilog(builder.Configuration);

builder.Services.AddScoped<ExceptionHandlingMiddleware>();

var app = builder.Build();
app.MapGrpcService<UserService>();

app.UseSerilog();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
app.UseGeneralConfigurations(app.Environment);
app.UseSwaggerExposer(app.Environment);
app.UseEventBusSubcribers();

try
{
    app.SeedAsync().Wait();
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
