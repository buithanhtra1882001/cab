using MediatR;
namespace CabNotificationService.Endpoints
{
    public static class EndpointExtension
    {
        public static RouteHandlerBuilder MapPost<T>(
            this IEndpointRouteBuilder endpoint, string route, string group)
            => endpoint.MapPost(route, async (T request, IMediator mediator)
                => await mediator.Send(request)).WithTags(group);

        public static RouteHandlerBuilder MapGet<T>(
            this IEndpointRouteBuilder endpoint, string route, string group, T request)
            => endpoint.MapGet(route, async (IMediator mediator)
                => await mediator.Send(request)).WithTags(group);

        public static RouteHandlerBuilder MapPut<T>(
           this IEndpointRouteBuilder endpoint, string route, string group)
           => endpoint.MapPut(route, async (T request, IMediator mediator)
               => await mediator.Send(request)).WithTags(group);

        public static RouteHandlerBuilder MapPatch<T>(
         this IEndpointRouteBuilder endpoint, string route, string group)
         => endpoint.MapMethods(route, new[] { "PATCH" }, async (T request, IMediator mediator)
             => await mediator.Send(request)).WithTags(group);
    }
}
