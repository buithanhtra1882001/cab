using Autofac.Extensions.DependencyInjection;
using CAB.BuildingBlocks.EventBusRegistration;
using CabPostService.Endpoints;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Helpers;
using CabPostService.Infrastructures.Middlewares;
using CabPostService.Infrastructures.Startup.PipelineExtensions;
using CabPostService.Infrastructures.Startup.ServicesExtensions;
using CabPostService.Services.Abstractions;
using CabUserService.Services;
using CabPostService.Jobs.Post;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using System.Net;
using TimeZoneConverter;
using Autofac.Core;
using CabPostService.EventHub;
using CabPostService.EventHub.Services;
using CabPostService.EventHub.Rabbit;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = CabPostService.Infrastructures.Loggings.LoggerFactory.CreateLogger(builder.Configuration);

builder.Services.AddGeneralConfigurations(builder.Configuration);
builder.Services.AddSwaggerService();
builder.Services.AddLazyCache();
builder.Services.AddInjectedServices();
builder.Services.AddGrpc();
builder.Services.AddGrpcConfigurations(builder.Configuration);
builder.Services.AddEventBus(builder.Configuration);
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IDonateService, DonateService>();
builder.Host
    .UseSerilog(CabPostService.Infrastructures.Loggings.LoggerFactory.SetupLogger)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.WebHost
    .UseUrls()
    .UseKestrel()
    .ConfigureKestrel(options =>
    {
        var gRPCPort = builder.Configuration.GetValue("GRPC_PORT", 10003);
        var httpPort = builder.Configuration.GetValue("PORT", 9003);

        options.Listen(IPAddress.Any, httpPort, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        });
        options.Listen(IPAddress.Any, gRPCPort, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http2;
        });
    })
    .UseContentRoot(Directory.GetCurrentDirectory());

builder.Services.AddHangfire(config =>
{
    config
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(builder.Configuration.GetValue<string>("PostDbConnectionString"));
    });

    var cronEveryMinute = "*/1 * * * *";
    var cron24hours = "0 0 * * *";
    var recurringJobOptions = new RecurringJobOptions
    {
        TimeZone = TZConvert.GetTimeZoneInfo("Etc/GMT+7")
    };

    //RecurringJob.AddOrUpdate<PointCalculateJob>("id-run-and-wait", x => x.CalculatePointAsync(), cronEveryMinute, recurringJobOptions);

    RecurringJob.AddOrUpdate<TrendingPointCalculateJob>("calculate-toptrending-posts", x => x.CalculateTrendingPointAsync(), cron24hours, recurringJobOptions);
});

builder.Services.AddHangfireServer();
builder.Services.AddScoped<ExceptionHandlingMiddleware>();
builder.Services.AddControllers();

// Handling config donate rabbit + signalR
builder.Services.AddSignalR();

builder.Services.AddHttpClient();
builder.Services.AddHostedService<RabbitMqListener>();
// End

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("http://192.168.1.233:8080", "https://devcab.org/", "http://devcab.org/")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();
app.UseCors("AllowSpecificOrigin");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseSwaggerExposer(app.Environment);
app.UseGeneralConfigurations(app.Environment);
app.UseEventBusSubcribers();

app.MapMinimalApiEndpoints();

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
