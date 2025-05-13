using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Exceptions;
using Swashbuckle.AspNetCore.Annotations;

namespace CabPostService.Endpoints
{
    public static class SharePostEndPoints
    {
        public const string Prefix = "/api/v1/posts/shares";
        private const string Group = "Post";
        private const string SharePostDesc = "Share A Post To Profile";
        private const string SharedPostsForUserDesc = "Share post for users by UserIds";
        private const string GetSharedPostByIdDesc = "Get Post shared By Id";

        public static void MapSharePostEndpoints(this IEndpointRouteBuilder endpoint)
        {
            // Get Post Share By User Id
            //endpoint.MapPost<GetSharePostByUserIdQuery>($"{Prefix}", Group)
            //  .Produces<PagingResponse<SharePostResponse>>()
            //  .WithMetadata(new SwaggerOperationAttribute(GetSharedPostsDesc, $"{GetSharedPostsDesc}."));

            endpoint.MapGet($"{Prefix}/" + "{postId:guid}",
                async (Guid postId, IMediator mediator)
                  => await mediator.Send(new GetSharePostByIdQuery() { Id = postId }))
              .WithTags(Group)
              .Produces<SharePostResponse>()
              .WithMetadata(new SwaggerOperationAttribute(GetSharedPostByIdDesc, $"{GetSharedPostByIdDesc}."));

            // Create Post Share
            endpoint.MapPost($"{Prefix}",
                async ([FromQuery] Guid auid, SharePostCommand request, IMediator mediator)
                  =>
                {
                    request.UserId = auid;
                    return await mediator.Send(request);
                })
              .WithTags(Group)
              .Produces<SharePostResponse>()
              .WithMetadata(new SwaggerOperationAttribute(SharePostDesc, $"{SharePostDesc}."));

            // Create Share Post For Users 
            endpoint.MapPost($"{Prefix}-post-users",
                async ([FromQuery] Guid auid, CreateSharePostForUsersCommand request, IMediator mediator)
                  =>
                {
                    request.UserId = auid;

                    try
                    {
                        return await mediator.Send(request);
                    }
                    catch
                    {
                        throw new OpenApiException("Error when trying share a post");
                    }
                })
              .WithTags(Group)
              .Produces<List<Guid>>()
              .WithMetadata(new SwaggerOperationAttribute(SharedPostsForUserDesc, $"{SharedPostsForUserDesc}."));
        }
    }
}
