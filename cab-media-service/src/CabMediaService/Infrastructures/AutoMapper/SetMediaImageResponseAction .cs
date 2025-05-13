using AutoMapper;
using CabMediaService.Models.Dtos;
using CabMediaService.Models.Entities;

namespace CabMediaService.Infrastructures.AutoMapper
{
    public class SetMediaImageResponseAction : IMappingAction<MediaImage, MediaImageResponse>
    {
        private readonly IConfiguration _configuration;

        public SetMediaImageResponseAction(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Process(MediaImage source, MediaImageResponse destination, ResolutionContext context)
        {
            var endpoint = _configuration.GetValue<string>("MinIOSettings:Endpoint");
            var bucketName = _configuration.GetValue<string>("MinIOSettings:BucketName");

            destination.Url = $"https://{endpoint}/{bucketName}/{source.FilePath}";
        }
    }
}