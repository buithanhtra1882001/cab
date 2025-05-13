using AutoMapper;
using WCABNetwork.Cab.IdentityService.Models.Dtos.Requests;
using WCABNetwork.Cab.IdentityService.Models.Entities;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterRequest, Account>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.Email));
        }
    }
}
