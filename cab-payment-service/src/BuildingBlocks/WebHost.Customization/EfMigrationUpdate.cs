﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using System.Data.SqlClient;

namespace WebHost.Customization
{
    public static class EfMigrationUpdate
    {
        public static bool IsInKubernetes(IServiceProvider services)
        {
            var cfg = services.GetService<IConfiguration>();
            var orchestratorType = cfg.GetValue<string>("OrchestratorType");
            return orchestratorType?.ToUpper() == "K8S";
        }

        public static void MigrateDbContext<TContext>(IServiceProvider services, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            var underK8s = IsInKubernetes(services);

            using (var scope = services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();
                var context = serviceProvider.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

                    if (underK8s)
                    {
                        InvokeSeeder(seeder, context, serviceProvider);
                    }
                    else
                    {
                        var retries = 10;
                        var retry = Policy.Handle<SqlException>()
                            .WaitAndRetry(
                                retryCount: retries,
                                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                onRetry: (exception, timeSpan, retry, ctx) =>
                                {
                                    logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", nameof(TContext), exception.GetType().Name, exception.Message, retry, retries);
                                });

                        //if the sql server container is not created on run docker compose this
                        //migration can't fail for network related exception. The retry options for DbContext only
                        //apply to transient exceptions
                        // Note that this is NOT applied when running some orchestrators (let the orchestrator to recreate the failing service)
                        retry.Execute(() => InvokeSeeder(seeder, context, serviceProvider));
                    }

                    logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                    if (underK8s)
                    {
                        throw;          // Rethrow under k8s because we rely on k8s to re-run the pod
                    }
                }
            }
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
            where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
