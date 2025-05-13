using CabMediaService.Services.Interfaces;

namespace CabMediaService.BackgroundServices
{
    public class CleanService : BackgroundService
    {
        private readonly ILogger<CleanService> _logger;
        private readonly IAWSMediaService _imageService;

        public CleanService(IAWSMediaService imageService, ILogger<CleanService> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var count = await _imageService.CleanAsync();
                    _logger.LogInformation("Finished cleaning unused images: {Count} items", count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error on CleanImageService.ExecuteAsync");
                }

                await Task.Delay(GetInterval(), cancellationToken);
            }
        }

        private TimeSpan GetInterval()
        {
            const int hours = 4;
            var now = DateTime.UtcNow;
            var executionTime = now.Date.AddHours(hours);

            if (executionTime < now)
            {
                executionTime = executionTime.AddDays(1);
            }

            return executionTime - now;
        }
    }
}