namespace CabNotificationService.Endpoints
{
    public static class MinimalApiEndpoints
    {
        public static void MapMinimalApiEndpoints(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapNotificationEndpoints();
        }
    }

}

