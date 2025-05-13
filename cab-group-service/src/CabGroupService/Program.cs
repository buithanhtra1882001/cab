using Autofac.Extensions.DependencyInjection;
using CAB.BuildingBlocks.EventBusRegistration;
using CabGroupService.Infrastructures.DbContexts;
using CabGroupService.Infrastructures.Middlewares;
using CabGroupService.Infrastructures.Startup.PipelineExtensions;
using CabGroupService.Infrastructures.Startup.ServicesExtensions;
using CabGroupService.Models.Dtos;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = CabGroupService.Infrastructures.Loggings.LoggerFactory.CreateLogger(builder.Configuration);

builder.Services.AddGeneralConfigurations(builder.Configuration);
builder.Services.AddSwaggerService();
builder.Services.AddLazyCache();
builder.Services.AddInjectedServices();
builder.Services.AddEventBus(builder.Configuration);
builder.Host
    .UseSerilog(CabGroupService.Infrastructures.Loggings.LoggerFactory.SetupLogger)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddScoped<ExceptionHandlingMiddleware>();
builder.Services.AddControllers();

// Handling config donate rabbit + signalR
builder.Services.AddSignalR();

builder.Services.AddHttpClient();

//Configuration FluentValidation
builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.ImplicitlyValidateChildProperties = true;
    config.DisableDataAnnotationsValidation = true;
});

builder.Services.AddValidatorsFromAssemblyContaining<RequestAddGroup>();
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

try
{
    app.MigrateDbContext<GroupDbContext>((_, __) => { }).Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
