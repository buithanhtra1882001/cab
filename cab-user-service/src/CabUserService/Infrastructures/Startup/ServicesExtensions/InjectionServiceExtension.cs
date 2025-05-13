using CAB.BuildingBlocks.EventBusRegistration;
using CabUserService.Clients;
using CabUserService.Clients.Interfaces;
using CabUserService.Infrastructures.Communications.Http;
using CabUserService.Infrastructures.DbContexts;
using CabUserService.Infrastructures.Repositories;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.IntegrationEvents.EventHandlers;
using CabUserService.Services;
using CabUserService.Services.Interfaces;
using MediatR;
using System.Reflection;

namespace CabUserService.Infrastructures.Startup.ServicesExtensions
{
    public static class InjectionServiceExtension
    {
        public static void AddInjectedServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddSingleton<CassandraDbContext>();
            services.AddSingleton<ScyllaDbContext>();
            services.AddDbContext<PostgresDbContext>();
            services.AddHttpClient<IHttpClientWrapper, HttpClientWrapper>();
            services.AddHttpContextAccessor();
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddTransient<IMediaClient, MediaClient>();

            services.AddTransient<IUserDetailRepository, UserDetailRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserImageRepository, UserImageRepository>();
            services.AddTransient<IUserFriendRepository, UserFriendRepository>();
            services.AddTransient<IUserCategoryRepository, UserCategoryRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IChatMessageRepository, ChatMessageRepository>();
            services.AddTransient<IChatMessageUserIdMaterializedViewRepository, ChatMessageUserIdMaterializedViewRepository>();
            services.AddTransient<IDonateReceiverRequestRepository, DonateReceiverRequestRepository>();

            #region internal service
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<IUserCategoryService, UserCategoryService>();
            services.AddTransient<IBalanceService, BalanceService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IDonateReceiverRequestService, DonateReceiverRequestService>();
            services.AddTransient<IWithdrawalRequestService, WithdrawalRequestService>();
            #endregion

            #region event bus handler
            services.AddTransient<UserCreatedIntegrationEventHandler>();
            services.AddTransient<UserRegisterIntegrationEventHandler>();
            services.AddEventBus(configuration);
            #endregion
        }
    }
}