using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace CabPostService.Endpoints
{
    public static class CommentEndpoints
    {
        private const string prefix = "/api/v1/comments";
        private const string group = "Comment";

        public static void MapCommentEndpoints(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapPost($"{prefix}/get-comment-by-postId",
             async ([FromQuery] Guid auid, GetCommentQuery request, IMediator mediator)
             =>
             {
                 request.UserId = auid;
                 return await mediator.Send(request);
             })
             .WithTags(group)
             .Produces<PostCommentPostIdMaterializedViewResponse>()
             .WithMetadata(new SwaggerOperationAttribute("Get comment of the post", "Get comment of the post."));

            endpoint.MapPost($"{prefix}/get-reply-by-commentId",
             async (GetReplyQuery request, IMediator mediator)
             =>
             {
                 return await mediator.Send(request);
             })
             .WithTags(group)
             .Produces<IList<UserReplyResponse>>()
             .WithMetadata(new SwaggerOperationAttribute("Get reply of the comment", "Get reply of the comment."));

            endpoint.MapPost($"{prefix}",
            async ([FromQuery] Guid auid, CommentCommand request, IMediator mediator) =>
            {
                request.UserId = auid;
                return await mediator.Send(request);
            })
             .WithTags(group)
             .Produces<Guid>()
             .WithMetadata(new SwaggerOperationAttribute("Comment on the post", "Comment on the post."));

            endpoint.MapPost($"{prefix}/reply",
            async ([FromQuery] Guid auid, ReplyCommand request, IMediator mediator) =>
            {
                request.UserId = auid;
                return await mediator.Send(request);
            })
             .WithTags(group)
             .Produces<Guid>()
             .WithMetadata(new SwaggerOperationAttribute("Reply a comment or reply other reply", "Reply a comment or reply other reply."));

            endpoint.MapPost($"{prefix}/image-comments",
            async ([FromQuery] Guid auid, CreateImageCommentCommand request, IMediator mediator) =>
            {
                request.UserId = auid;
                return await mediator.Send(request);
            })
             .WithTags(group)
             .Produces<Guid>()
             .WithMetadata(new SwaggerOperationAttribute("Comment on the image", "Comment on the image."));

            endpoint.MapPatch<VoteUpCommentCommand>($"{prefix}/votes/up", group)
                .Produces<bool>()
                .WithMetadata(new SwaggerOperationAttribute("Up vote a comment", "Up vote a comment."));

            endpoint.MapPatch<VoteDownCommentCommand>($"{prefix}/votes/down", group)
                .Produces<bool>()
                .WithMetadata(new SwaggerOperationAttribute("Down vote a comment", "Down vote a comment."));

            endpoint.MapPut($"{prefix}/like-toggle",
               async ([FromQuery] Guid auid, LikeOrUnlikeCommentCommand request, IMediator mediator)
               =>
               {
                   request.UserId = auid;
                   return await mediator.Send(request);
               })
               .WithTags(group)
               .Produces<bool>()
               .WithMetadata(new SwaggerOperationAttribute("Like or unlike a comment", "Like or unlike a comment."));
        }
    }
}
