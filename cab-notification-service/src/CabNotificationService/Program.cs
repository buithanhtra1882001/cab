using Autofac.Extensions.DependencyInjection;
using CAB.BuildingBlocks.EventBusRegistration;
using CabNotificationService.Endpoints;
using CabNotificationService.Infrastructures.DbContexts;
using CabNotificationService.Infrastructures.Middlewares;
using CabNotificationService.Infrastructures.Startup.PipelineExtensions;
using CabNotificationService.Infrastructures.Startup.ServicesExtensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = CabNotificationService.Infrastructures.Loggings.LoggerFactory.CreateLogger(builder.Configuration);

builder.Services.AddGeneralConfigurations(builder.Configuration);
builder.Services.AddSwaggerService();
builder.Services.AddLazyCache();
builder.Services.AddInjectedServices();
builder.Services.AddEventBus(builder.Configuration);
builder.Host
    .UseSerilog(CabNotificationService.Infrastructures.Loggings.LoggerFactory.SetupLogger)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddScoped<ExceptionHandlingMiddleware>();
builder.Services.AddControllers();

// Handling config donate rabbit + signalR
builder.Services.AddSignalR();

builder.Services.AddHttpClient();
// End

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("http://192.168.1.233:8080", "https://devcab.org/",
            "http://devcab.org/", "https://api.devcab.org/")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

app.UseSwaggerExposer(app.Environment);
app.UseGeneralConfigurations(app.Environment);
app.UseEventBusSubcribers();

app.MapMinimalApiEndpoints();

try
{
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
