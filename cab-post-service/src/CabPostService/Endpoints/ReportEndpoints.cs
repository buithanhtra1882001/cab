using CabPostService.Models.Commands;
using CabPostService.Models.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CabPostService.Endpoints
{
    public static class ReportEndpoints
    {
        private const string prefix = "/api/v1/report";
        private const string group = "Report";

        public static void MapReportEndpoints(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapPost($"{prefix}",
                async ([FromQuery] Guid auid,ReportCommand request, IMediator mediator)
                =>
                {
                    request.UserId = auid;
                    return await mediator.Send(request);
                })
                .WithTags(group)
                .Produces<Guid>()
                .WithMetadata(new SwaggerOperationAttribute("Report a post", "Report a post."));
        }
    }
}