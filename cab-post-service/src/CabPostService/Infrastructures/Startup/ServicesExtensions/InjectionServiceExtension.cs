using CabPostService.Clients;
using CabPostService.Clients.Interfaces;
using CabPostService.Grpc.Procedures;
using CabPostService.Handlers.Post;

using CabPostService.Handlers.UserBehavior;

using CabPostService.Infrastructures.Communications.Http;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.IntegrationEvents.EventHandlers;

namespace CabPostService.Infrastructures.Startup.ServicesExtensions
{
    public static class InjectionServiceExtension
    {
        public static void AddInjectedServices(this IServiceCollection services)
        {
            //services.AddSingleton<CassandraDbContext>();
            services.AddSingleton<ScyllaDbContext>();
            services.AddDbContext<PostgresDbContext>();
            services.AddHttpClient<IHttpClientWrapper, HttpClientWrapper>();

            services.AddTransient<IMediaClient, MediaClient>();

            services.AddTransient<IPostCommentRepository, PostCommentRepository>();
            services.AddTransient<IPostCommentReplyRepository, PostCommentReplyRepository>();
            services.AddTransient<IImageCommentRepository, ImageCommentRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<IPostUserRepository, PostUserRepository>();
            services.AddTransient<IPostVideoRepository, PostVideoRepository>();
            services.AddTransient<IPostCategoryRepository, PostCategoryRepository>();
            services.AddTransient<IPostDonateRepository, PostDonateRepository>();
            services.AddTransient<IPostReportRepository, PostReportRepository>();
            services.AddTransient<IPostCommentPostIdMaterializedViewRepository, PostCommentPostIdMaterializedViewRepository>();
            services.AddTransient<IPostCommentReplyCommentIdMaterializedViewRepository, PostCommentReplyCommentIdMaterializedViewRepository>();
            services.AddTransient<ISequenceGeneratorRepository, SequenceGeneratorRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IPostImageRepository, PostImageRepository>();
            services.AddTransient<ISharePostRepository, SharePostRepository>();
            services.AddTransient<IUserBahaviorRepository, UserBahaviorRepository>();
            services.AddTransient<ICommentLikeRepository, CommentLikeRepository>();

            services.AddTransient<UserProfileCreatedIntegrationEventHandler>();
            services.AddTransient<UserProfileUpdatedIntegrationEventHandler>();


            services.AddTransient<PostHandler>();
            services.AddTransient<UserProfileUpdateAvatarIntegrationEventHandler>();
            services.AddTransient<UserBehaviorHandler>();


            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IMediaService, MediaService>();
            services.AddTransient<IPostNotifyAdminRepository, PostNotifyAdminRepository>();
            services.AddTransient<IPostHashtagRepository, PostHashtagRepository>();
        }
    }
}