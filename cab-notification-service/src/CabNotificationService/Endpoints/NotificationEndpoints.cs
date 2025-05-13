using CabNotificationService.Models.Commands;
using CabNotificationService.Models.Dtos;
using CabNotificationService.Models.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CabNotificationService.Endpoints
{
    public static class NotificationEndpoints
    {
        private const string prefix = "/api/v1/notifications";
        private const string group = "Notification";
        public static void MapNotificationEndpoints(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapPost($"{prefix}/user-notifications",
             async ([FromQuery] Guid auid, GetNotificationQuery request, IMediator mediator)
             =>
             {
                 request.UserId = auid;
                 return await mediator.Send(request);
             })
             .WithTags(group)
             .Produces<PagingResponse<NotificationResponse>>()
             .WithMetadata(new SwaggerOperationAttribute("Get notifications by userid", "Get notifications by userid."));

            endpoint.MapPut($"{prefix}/toggle-read",
             async ([FromQuery] Guid auid, ToggleNotificationReadStatusCommand request, IMediator mediator)
             =>
             {
                 request.UserId = auid;
                 return await mediator.Send(request);
             })
             .WithTags(group)
             .Produces<bool>()
             .WithMetadata(new SwaggerOperationAttribute("Toggle notification read status", "Toggle notification read status."));

            endpoint.MapPost($"{prefix}/mark-all",
             async ([FromQuery] Guid auid, MarkAllNotificationCommand request, IMediator mediator)
             =>
             {
                 request.UserId = auid;
                 return await mediator.Send(request);
             })
             .WithTags(group)
             .Produces<bool>()
             .WithMetadata(new SwaggerOperationAttribute("Mark all notifications", "Mark all notifications."));

        }
    }
}
