namespace CabUserService.Infrastructures.Startup.HangfireExtensions
{
    using CabUserService.Services;
    using Hangfire;

    public static class HangfireExtensions
    {
        [Obsolete]
        public static void ConfigureHangfire(this IApplicationBuilder app)
        {
            var hangfireServiceName = "UserService";
            var workerCount = Environment.ProcessorCount * 5;
            var pollingInterval = TimeSpan.FromSeconds(5);

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                ServerName = hangfireServiceName,
                WorkerCount = workerCount,
                SchedulePollingInterval = pollingInterval
            });
            RecurringJob.AddOrUpdate<UserService>(
                "send-email-user-creator",
                x => x.NotifyUserCreatorsAsync(),
                Cron.Daily(11,27),TimeZoneInfo.Local);
        }
    }
}
