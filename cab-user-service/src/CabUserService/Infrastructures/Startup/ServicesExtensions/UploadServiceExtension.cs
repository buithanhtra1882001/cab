using Microsoft.AspNetCore.Http.Features;

namespace CabUserService.Infrastructures.Startup.ServicesExtensions
{
    public static class UploadServiceExtension
    {
        public static void AddUploadServices(this IServiceCollection services)
        {
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
        }
    }
}
