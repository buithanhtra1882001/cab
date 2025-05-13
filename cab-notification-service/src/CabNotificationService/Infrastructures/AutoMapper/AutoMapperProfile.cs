using AutoMapper;
using CabNotificationService.Models.Dtos;
using CabNotificationService.Models.Entities;

namespace CabNotificationService.Infrastructures.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<NotificationUserIdMaterializedView, NotificationResponse>();
            CreateMap<NotificationResponse, Notification>().ReverseMap();
        }
    }
}
