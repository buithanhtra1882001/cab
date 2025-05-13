using AutoMapper;
using CabPostService.Grpc.Protos.PostServer;
using CabPostService.Grpc.Protos.UserClient;
using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using CabPostService.Models.Queries;

namespace CabPostService.Infrastructures.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PostCategory, UserPostCategoryResponse>();

            CreateMap<CreatePostCategoryCommand, PostCategory>();
            CreateMap<Post, UserPostResponse>();
            CreateMap<Post, PostResponse>();
            CreateMap<Post, AdminGetPostsResponse>();
            CreateMap<Post, GetAllPostResponse>();
            CreateMap<PostComment, UserCommentResponse>();
            CreateMap<PostCommentPostIdMaterializedView, UserCommentResponse>();
            CreateMap<UserModel, UserResponse>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToDateTime()))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(x => x.UpdatedAt.ToDateTime()));
            CreateMap<PostCommentReply, UserReplyResponse>();
            CreateMap<UserReplyResponse, PostCommentReply>();
            CreateMap<PostCommentReplyCommentIdMaterializedView, UserReplyResponse>();
            CreateMap<PostMiniUser, CreatorResponse>()
                 .ForMember(dest => dest.UserId, opt => opt.MapFrom(x => x.Id));

            //mediator command, query mapper response
            CreateMap<ReportCommand, PostReport>();

            CreateMap<DonateCommand, PostDonate>()
                .ForMember(dest => dest.DonaterId, opt => opt.MapFrom(x => x.Auid));

            CreateMap<CreatePostCommand, Post>();

            CreateMap<GetAllPostFilterCommand, GetAllPostFilter>();
            CreateMap<GetAllPostOrderByPointWithPagingQuery, GetAllPostOrderByPointWithPaging>();
            CreateMap<GetPostVideosQuery, GetPostVideosOrderByCreatedWithPaging>();
            CreateMap<CommentCommand, PostComment>();
            CreateMap<CreateImageCommentCommand, ImageComment>();

            CreateMap<EditPostCommand, Post>();

            CreateMap<SharePost, SharePostResponse>();
            CreateMap<SharePostCommand, SharePost>();
            CreateMap<GetSharePostByUserIdQuery, SharePostFilter>();

            CreateMap<CreatePostVideoCommand, PostVideo>();
            CreateMap<User, PostMiniUser>().ReverseMap();

            
            CreateMap<UserBehaviorRequest, UserBehavior>();
            CreateMap<PostHashtag, HashtagResponse>();
        }
    }
}
