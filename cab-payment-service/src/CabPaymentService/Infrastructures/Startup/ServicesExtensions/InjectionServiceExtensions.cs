using CabPaymentService.Infrastructures.Communications;
using CabPaymentService.Infrastructures.Configurations;
using CabPaymentService.Infrastructures.DbContexts;
using CabPaymentService.Infrastructures.Helpers;
using CabPaymentService.Infrastructures.Repositories;
using CabPaymentService.Infrastructures.Repositories.Interfaces;
using CabPaymentService.Services;
using CabPaymentService.Services.Interfaces;

namespace CabPaymentService.Infrastructures.Startup.ServicesExtensions
{
    public static class InjectionServiceExtensions
    {
        public static void AddInjectedServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddSingleton<CassandraDbContext>();
            services.AddDbContext<PostgresDbContext>();
            services.AddHttpClient<IHttpClientWrapper, HttpClientWrapper>().ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
            });

            var vnPayConfig = configuration.GetSection("VnPay").Get<VnPayConfiguration>();

            var commissionConfig = configuration.GetSection("DonateCommissionPercentage").Get<CommissionConfig>();
            
            services.AddSingleton(vnPayConfig);
            
            services.AddSingleton(commissionConfig);

            services.AddTransient<IVnPayTransactionRepository, VnPayTransactionRepository>();

            services.AddTransient<IVnPayService, VnPayService>();
        }
    }
}
