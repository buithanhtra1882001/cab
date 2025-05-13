using AutoMapper;
using CabPaymentService.Model.Dtos;
using CabPaymentService.Model.Entities;

namespace CabPaymentService.Infrastructures.AutoMapper
{
    public class CabPaymentMapperProfile : Profile
    {
        public CabPaymentMapperProfile()
        {
            CreateMap<OrderInfoDto, VnPayTransaction>()
                .ForMember(e => e.Id, opt => opt.MapFrom(dto => dto.OrderId));
            CreateMap<BillInfoDto, BillInfo>();
            CreateMap<InvoiceDto, Invoice>();
        }
    }
}
