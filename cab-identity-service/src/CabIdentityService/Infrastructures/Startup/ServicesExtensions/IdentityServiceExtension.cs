using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WCABNetwork.Cab.IdentityService.Infrastructures.DbContexts;
using WCABNetwork.Cab.IdentityService.Models.Entities;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Collections.Generic;
using System;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Startup.ServicesExtensions
{
    public static class IdentityServiceExtension
    {
        public static readonly string RESOURCE_PATH = "Resources";

        public static void AddIdentityCore(this IServiceCollection services)
        {
            services.AddLocalization(opts => { opts.ResourcesPath = RESOURCE_PATH; });

            services.AddIdentity<Account, IdentityRole<int>>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = System.TimeSpan.FromMinutes(5);
                })
                .AddEntityFrameworkStores<IdentityCoreDbContext>()
                .AddDefaultTokenProviders()
                .AddUserManager<UserManager<Account>>()
                .AddRoleManager<RoleManager<IdentityRole<int>>>();
            services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(5));
        }
    }
}
