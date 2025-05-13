using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using WCABNetwork.Cab.IdentityService.Infrastructures.DbContexts;
using WCABNetwork.Cab.IdentityService.Infrastructures.Repositories;
using WCABNetwork.Cab.IdentityService.Infrastructures.Repositories.Interfaces;
using WCABNetwork.Cab.IdentityService.Infrastructures.Token;
using WCABNetwork.Cab.IdentityService.IntegrationEvents.EventHandlers;
using WCABNetwork.Cab.IdentityService.Services.Implements;
using WCABNetwork.Cab.IdentityService.Services.Interfaces;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Startup.ServicesExtensions
{
    public static class InjectionServiceExtension
    {
        public static void AddInjectedServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IdentityCoreDbContext>();
            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddTransient<IUserRefreshTokenRepository, UserRefreshTokenRepository>();
            // Services
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IStringLocalizer<AccountService>, StringLocalizer<AccountService>>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IFingerprintService, FingerprintService>();
            services.AddTransient<UserDeletedIntegrationEventHandler>();
            services.AddTransient<UserRegisterIntegrationEventHandler>();
        }
    }
}