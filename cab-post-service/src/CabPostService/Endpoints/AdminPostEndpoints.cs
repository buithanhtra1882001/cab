using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace CabPostService.Endpoints
{
    public static class AdminPostEndpoints
    {
        private const string prefix = "/api/v1/admin/posts";
        private const string group = "PostAdmin";

        public static void MapAdminPostEndpoints(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapGet($"{prefix}/" + "{postId}",
                async (string postId, IMediator mediator)
                => await mediator.Send(new GetPostByIdQuery { PostId = postId }))
                .WithTags(group)
                .Produces<UserPostResponse>()
                .WithMetadata(new SwaggerOperationAttribute("Admin get post by Id", "Admin get post by Id."));

            endpoint.MapPost<GetAllPostFilterCommand>($"{prefix}/search", group)
                .Produces<PagingResponse<AdminGetPostsResponse>>()
                .WithMetadata(new SwaggerOperationAttribute("Admin get posts", "Admin get posts."));

            endpoint.MapDelete($"{prefix}/" + "{postId}",
                async (string postId, IMediator mediator) =>
                {
                    return await mediator.Send(new DeletePostCommand { PostId = postId });
                }).WithTags(group)
                .Produces<bool>()
                .WithMetadata(new SwaggerOperationAttribute("Admin delete post by id", "Admin delete post by id."));

            endpoint.MapPatch<LockPostCommand>($"{prefix}/lock", group)
               .Produces<bool>()
               .WithMetadata(new SwaggerOperationAttribute("Admin lock post by Id", "Admin lock post by Id."));

            //category
            endpoint.MapPost<CreatePostCategoryCommand>($"{prefix}/category", group)
              .Produces<Guid>()
              .WithMetadata(new SwaggerOperationAttribute("Admin create post category", "Admin create post category."));

            endpoint.MapDelete($"{prefix}/categories" + "{id}",
                async (Guid id, IMediator mediator) =>
                {
                    return await mediator.Send(new DeletePostCategoryCommand { PostCategoryId = id });
                }).WithTags(group)
                .Produces<bool>()
                .WithMetadata(new SwaggerOperationAttribute("Admin delete post category by id", "Admin delete post category by id."));
        }
    }
}

