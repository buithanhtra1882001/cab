namespace CabPostService.Endpoints
{
    public static class MinimalApiEndpoints
    {
        public static void MapMinimalApiEndpoints(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapPostEndpoints();
            endpoint.MapPostVideoEndpoints();
            endpoint.MapPostCategoryEndpoints();
            endpoint.MapDonateEndpoints();
            endpoint.MapReportEndpoints();
            endpoint.MapCommentEndpoints();
            endpoint.MapAdminPostEndpoints();
            endpoint.MapSharePostEndpoints();
        }
    }
}

