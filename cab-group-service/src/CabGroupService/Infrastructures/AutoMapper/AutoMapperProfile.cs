using AutoMapper;
using CabGroupService.Models.Dtos;
using CabGroupService.Models.Dtos.Group;
using CabGroupService.Models.Entities;

namespace CabGroupService.Infrastructures.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<NotificationUserIdMaterializedView, NotificationResponse>();
            CreateMap<NotificationResponse, Notification>().ReverseMap();

            CreateMap<Group, GroupResponse>().ReverseMap();
            CreateMap<Group, RequestAddGroup>().ReverseMap();
            CreateMap<Group, RequestUpdateGroup>().ReverseMap();
            CreateMap<GroupMembers, RequestJoinGroup>().ReverseMap();
            CreateMap<GroupMembers, GroupMemberPendingApproval>().ReverseMap();
        }
    }
}
