using AutoMapper;
using CabUserService.Grpc.Profos.UserServer;
using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;
using Google.Protobuf.WellKnownTypes;

namespace CabUserService.Infrastructures.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserDetail, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.UserId));

            CreateMap<UserCreateUpdateRequest, UserDetail>()
                .ForMember(dest => dest.School, opt => opt.MapFrom(x => string.Join(",", x.Schools)))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(x => string.Join(",", x.Companys)));
            CreateMap<UserDetail, UserModel>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(x => Timestamp.FromDateTime(DateTime.SpecifyKind(x.CreatedAt, DateTimeKind.Utc))))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(x => Timestamp.FromDateTime(DateTime.SpecifyKind(x.UpdatedAt, DateTimeKind.Utc))));
            CreateMap<User, UserModel>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(x => Timestamp.FromDateTime(DateTime.SpecifyKind(x.CreatedAt, DateTimeKind.Utc))))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(x => Timestamp.FromDateTime(DateTime.SpecifyKind(x.UpdatedAt, DateTimeKind.Utc))));
            CreateMap<UserDetail, PublicUserInformationResponse>();
            CreateMap<UserDetail, FullUserInformationResponse>()
                .ForMember(dest => dest.Schools, opt => opt.MapFrom(x => x.School.Split(",", StringSplitOptions.None)))
                .ForMember(dest => dest.Companys, opt => opt.MapFrom(x => x.Company.Split(",", StringSplitOptions.None)));
            CreateMap<User, PublicUserInformationResponse>();
            CreateMap<User, FullUserInformationResponse>();
            //CreateMap<UserDetail, UserModel>();
            CreateMap<User, UserFindByUserNameResponse>();
            CreateMap<User, CreatorResponse>();
            CreateMap<Category, CategoryResponse>();

            CreateMap<AddUserTransactionRequest, UserTransaction>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => Guid.NewGuid()));

            CreateMap<UserTransaction, UserTransactionDto>();
            CreateMap<UserBalanceLog, UserBalanceLogDto>();
            CreateMap<RequestFriendRequest, UserRequestFriendAction>();

            CreateMap<CreateMessageRequest, ChatMessage>();
            CreateMap<ChatMessage, MessagesResponse>();
            CreateMap<ChatMessage, UserMessageResponse>();

            CreateMap<DonateReceiverRequestDto, DonateReceiverRequest>();
            CreateMap<DonateReceiverRequest, DonateReceiverRequestResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(x => x.User.Id))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(x => Timestamp.FromDateTime(DateTime.SpecifyKind(x.CreatedAt, DateTimeKind.Utc))))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(x => Timestamp.FromDateTime(DateTime.SpecifyKind(x.UpdatedAt, DateTimeKind.Utc))));

            CreateMap<WithdrawalRequestDto, WithdrawalRequest>();
            CreateMap<WithdrawalRequest, WithdrawalRequestResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(x => x.User.Id));
        }
    }
}
