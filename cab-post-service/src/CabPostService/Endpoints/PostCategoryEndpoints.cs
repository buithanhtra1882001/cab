using CabPostService.Constants;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace CabPostService.Endpoints
{
    public static class PostCategoryEnpoints
    {
        private const string prefix = "/api/v1/postcategories";
        private const string group = "PostCategory";
        public static void MapPostCategoryEndpoints(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapGet($"{prefix}", group, new GetPostCategoryQuery())
                .Produces<IList<UserPostCategoryResponse>>()
                .WithMetadata(new SwaggerOperationAttribute("Get post categories", "Get post categories."));

            endpoint.MapGet($"{prefix}/" + "{slug}",
             async (string slug, IMediator mediator)
             => await mediator.Send(new GetPostCategoryBySlugQuery { Slug = slug }))
             .WithTags(group)
             .Produces<UserPostCategoryResponse>()
             .WithMetadata(new SwaggerOperationAttribute("Get Post category by Slug", "Get Post category by Slug."));

            endpoint.MapGet($"{prefix}/" + "{type}",
            async (int type, IMediator mediator)
            => await mediator.Send(new GetPostCategoryByTypeQuery { Type = type }))
            .WithTags(group)
            .Produces<IList<UserPostCategoryResponse>>()
            .WithMetadata(new SwaggerOperationAttribute("Get Post category by type: 1 - image, 2 - video", "Get Post category by type: 1 - image, 2 - video."));
        }
    }
}
