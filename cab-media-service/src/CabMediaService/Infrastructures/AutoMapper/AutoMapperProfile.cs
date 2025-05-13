using AutoMapper;
using CabMediaService.Models.Dtos;
using CabMediaService.Models.Entities;

namespace CabMediaService.Infrastructures.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MediaImage, MediaImageResponse>()
                .AfterMap<SetMediaImageResponseAction>();
        }
    }
}