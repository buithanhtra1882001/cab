namespace CabPostService.Jobs.Base
{
    public abstract class BaseJob<T>
    {
        protected IServiceProvider _serviceProvider;
        protected ILogger<T> _logger;

        protected BaseJob(
            IServiceProvider serviceProvider,
            ILogger<T> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
    }
}