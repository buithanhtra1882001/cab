using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CabPostService.Endpoints
{
    public static class PostVideoEndpoints
    {
        private const string prefix = "/api/v1/postvideos";
        private const string group = "Post Video";

        public static void MapPostVideoEndpoints(this IEndpointRouteBuilder endpoint)
        {
            // tam comment ko xoas
            // endpoint.MapPost($"{prefix}",
            //     async ([FromQuery] Guid auid, CreatePostVideoCommand request, IMediator mediator)
            //     =>
            //     {
            //         request.UserId = auid;
            //         return await mediator.Send(request);
            //     })
            //     .WithTags(group)
            //     .Produces<string>()
            //     .WithMetadata(new SwaggerOperationAttribute("Create new Post Video", "Create new Post Video."));
            
            endpoint.MapPost($"{prefix}/upload-video",
                    async ([FromQuery] Guid auid, IMediator mediator, HttpRequest request)
                        =>
                    {
                        var files = request.Form.Files;

                        if (files is null || files.Count == 0)
                        {
                            var result = new BaseResponse
                            {
                                IsSuccess = false,
                                Message = "Vui lòng chọn file tải lên"
                            };
                            return result;
                        }
                        var uploadFileCommand = new UploadFileCommand
                        {
                            Files = files,
                            UserId = auid
                        };
                        return await mediator.Send(uploadFileCommand);
                    })
                .AllowAnonymous()
                .WithTags(group)
                .Produces<BaseResponse>()
                .WithMetadata(new SwaggerOperationAttribute("Create new Post Video", "Create new Post Video."));
            
            endpoint.MapPost($"{prefix}/update-avtp",
                    async ([FromQuery] Guid auid, UpdateAVTPCommand request, IMediator mediator)
                        =>
                    {
                        return await mediator.Send(request);
                    })
                .AllowAnonymous()
                .WithTags(group)
                .Produces<BaseResponse>()
                .WithMetadata(new SwaggerOperationAttribute("Update AVTP Post Video", "Update AVTP Post Video."));

            endpoint.MapPost($"{prefix}/post-videos",
                async ([FromQuery] Guid auid, GetPostVideosQuery request, IMediator mediator)
                =>
                {
                    request.UserId = auid;
                    return await mediator.Send(request);
                })
                .WithTags(group)
                .Produces<IList<GetAllPostResponse>>()
                .WithMetadata(new SwaggerOperationAttribute("Get post videos", "Get post videos."));

            endpoint.MapPost($"{prefix}/get-latest-videos",
               async ([FromQuery] Guid auid, GetLatestVideosQuery request, IMediator mediator)
               =>
               {
                   request.UserId = auid;
                   return await mediator.Send(request);
               })
               .WithTags(group)
               .Produces<PagingResponse<GetAllPostResponse>>()
               .WithMetadata(new SwaggerOperationAttribute("Get latest videos", "Get latest videos."));
        }
    }
}

