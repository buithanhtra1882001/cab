using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using CabPostService.Models.Dtos.Hashtag;
using CabPostService.Models.Entities;
using CabPostService.Models.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CabPostService.Endpoints
{
    public static class PostEndpoints
    {
        private const string prefix = "/api/v1/posts";
        private const string group = "Post";

        public static void MapPostEndpoints(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapGet($"{prefix}/" + "{postId}",
                async (string postId, IMediator mediator)
                => await mediator.Send(new GetPostByIdQuery { PostId = postId }))
                .WithTags(group)
                .Produces<UserPostResponse>()
                .WithMetadata(new SwaggerOperationAttribute("Get Post by Id", "Get Post by Id."));

            endpoint.MapGet($"{prefix}",
               async (string slug, IMediator mediator)
               => await mediator.Send(new GetPostBySlugQuery { Slug = slug }))
               .WithTags(group)
               .Produces<UserPostResponse>()
               .WithMetadata(new SwaggerOperationAttribute("Get Post by Slug", "Get Post by Slug."));

            endpoint.MapGet($"{prefix}/get-hashtag/" + "{totalRecord}",
                   async (int totalRecord, IMediator mediator) =>
                   {
                       return await mediator.Send(new GetHashtagByTotalRecordQuery { TotalRecord = totalRecord, Type = "GET" });
                   }).WithTags(group)
               .Produces<List<HashtagResponse>>()
               .WithMetadata(new SwaggerOperationAttribute("Get hashtag for post", "Get hashtag for post"));

            endpoint.MapPost($"{prefix}/home-post",
                async ([FromQuery] Guid auid, GetAllPostOrderByPointWithPagingQuery request, IMediator mediator)
                =>
                {
                    request.UserId = auid;
                    return await mediator.Send(request);
                })
                .WithTags(group)
                .Produces<IList<GetAllPostResponse>>()
                .WithMetadata(new SwaggerOperationAttribute("Get all post ordered by point with paging", "Get all post ordered by point with paging."));

            endpoint.MapPost($"{prefix}/get-posts-by-user",
                async (GetPostByUserIdQuery request, IMediator mediator)
                =>
                {
                    return await mediator.Send(request);
                })
                .WithTags(group)
                .Produces<IList<GetAllPostResponse>>()
                .WithMetadata(new SwaggerOperationAttribute("Get posts by userId", "Get posts by userId."));

            endpoint.MapPost($"{prefix}/user/interacted-posts",
                async ([FromQuery] Guid auid, IMediator mediator)
                => await mediator.Send(new GetUserInteractedPostsQuery { UserId = auid }))
                .WithTags(group)
                .Produces<IList<PostInteractionResponse>>()
                .WithMetadata(new SwaggerOperationAttribute("Get User Interacted Posts", "Get User Interacted Posts."));

            endpoint.MapPost($"{prefix}/user/hide-behavior",
                async ([FromQuery] Guid auid, UpdateUserBehaviorHiddenCommand request, IMediator mediator)
                =>
                {
                    request.UserId = auid;
                    return await mediator.Send(request);
                })
                .WithTags(group)
                .Produces<bool>()
                .WithMetadata(new SwaggerOperationAttribute(" Update UserBehavior Hidden", "Update UserBehavior Hidden."));

            endpoint.MapPost($"{prefix}/get-creators",
               async ([FromQuery] Guid auid, GetCreatorByIdQuery request, IMediator mediator)
              =>
               {
                   var result = await mediator.Send(new GetCreatorByIdQuery
                   {
                       UserId = auid,
                       CreatorIds = request.CreatorIds,
                   });
                   return result;
               })
               .WithTags(group)
               .Produces<IList<CreatorResponse>>()
               .WithMetadata(new SwaggerOperationAttribute("Get Creator by Id", "Get Creator by Id."));

            endpoint.MapPost($"{prefix}",
                async ([FromQuery] Guid auid, CreatePostCommand request, IMediator mediator)
                =>
                {
                    request.UserId = auid;
                    return await mediator.Send(request);
                })
                .WithTags(group)
                .Produces<string>()
                .WithMetadata(new SwaggerOperationAttribute("Create new Post", "Create new Post."));

            endpoint.MapPost($"{prefix}/spam/" + "{postId}",
                    async ([FromQuery] Guid auid, string postId, IMediator mediator) =>
                    {
                        return await mediator.Send(new SpamPostCommand { PostId = postId, UserId = auid });
                    }).WithTags(group)
                .Produces<bool>()
                .WithMetadata(new SwaggerOperationAttribute("Spam Post by User", "Spam Post by User."));

            endpoint.MapPost($"{prefix}/update-notify-admin/",
                    async ([FromQuery] Guid auid, UpdateNotifyAdminCommand request, IMediator mediator) =>
                    {
                        return await mediator.Send(new UpdateNotifyAdminCommand { IdNotify = request.IdNotify, IsAcceptHide = request.IsAcceptHide });
                    }).WithTags(group)
                .Produces<bool>()
                .WithMetadata(new SwaggerOperationAttribute("Update Notify Admin", "Update Notify Admin."));

            endpoint.MapPost($"{prefix}/search-hashtag",
                   async (SearchHashtagDTO searchHashtag, IMediator mediator) =>
                   {
                       return await mediator.Send(new GetHashtagByTotalRecordQuery { Keyword = searchHashtag.Keyword, TotalRecord = searchHashtag.TotalRecord, Type = "SEARCH" });
                   }).WithTags(group)
               .Produces<List<HashtagResponse>>()
               .WithMetadata(new SwaggerOperationAttribute("Search hashtag for post", "Search hashtag for post"));

            endpoint.MapPost($"{prefix}/get-videos-suggestion",
               async ([FromQuery] Guid auid, GetVideoSuggestQuery request, IMediator mediator)
               =>
               {
                   request.UserId = auid;
                   return await mediator.Send(request);
               })
               .WithTags(group)
               .Produces<IList<GetAllPostResponse>>()
               .WithMetadata(new SwaggerOperationAttribute("Get video suggestions", "Get video suggestions."));

            endpoint.MapPost($"{prefix}/get-top-trendings",
               async ([FromQuery] Guid auid, GetTopTrendingQuery request, IMediator mediator)
               =>
               {
                   request.UserId = auid;
                   return await mediator.Send(request);
               })
               .WithTags(group)
               .Produces<IList<GetAllPostResponse>>()
               .WithMetadata(new SwaggerOperationAttribute("Get posts top trending", "Get posts top trending."));

            endpoint.MapPut($"{prefix}",
                async ([FromQuery] Guid auid, EditPostCommand request, IMediator mediator)
                =>
                {
                    request.UserId = auid;
                    return await mediator.Send(request);
                })
                .WithTags(group)
                .Produces<bool>()
                .WithMetadata(new SwaggerOperationAttribute("Edit old post", "Edit old post."));

            endpoint.MapPut($"{prefix}/post/up",
               async ([FromQuery] Guid auid, VoteUpPostCommand request, IMediator mediator)
               =>
               {
                   request.UserId = auid;
                   return await mediator.Send(request);
               })
               .WithTags(group)
               .Produces<bool>()
               .WithMetadata(new SwaggerOperationAttribute("Vote Up a post", "Vote Up a post."));

            endpoint.MapPut($"{prefix}/post/down",
               async ([FromQuery] Guid auid, VoteDownPostCommand request, IMediator mediator)
               =>
               {
                   request.UserId = auid;
                   return await mediator.Send(request);
               })
               .WithTags(group)
               .Produces<bool>()
               .WithMetadata(new SwaggerOperationAttribute("Vote Down a post", "Vote Down a post."));

            endpoint.MapPut($"{prefix}/increase-view-count",
               async (IncreaseViewPostCommand request, IMediator mediator)
               =>
               {
                   return await mediator.Send(request);
               })
               .WithTags(group)
               .Produces<long>()
               .WithMetadata(new SwaggerOperationAttribute("Tracking: Increase view of post", "Tracking: Increase view of post."));

            endpoint.MapDelete($"{prefix}/" + "{postId}",
               async ([FromQuery] Guid auid, string postId, IMediator mediator) =>
               {
                   return await mediator.Send(new DeletePostCommand { PostId = postId, UserId = auid });
               }).WithTags(group)
               .Produces<bool>()
               .WithMetadata(new SwaggerOperationAttribute("Delete Post by Id", "Delete Post by Id."));
        }
    }
}

