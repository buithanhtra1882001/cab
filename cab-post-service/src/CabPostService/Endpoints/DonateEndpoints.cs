using CabPostService.Models.Commands;
using Swashbuckle.AspNetCore.Annotations;

namespace CabPostService.Endpoints
{
    public static class DonateEndpoints
    {
        private const string prefix = "/api/v1/donate";
        private const string group = "Donate";

        public static void MapDonateEndpoints(this IEndpointRouteBuilder endpoint)
        {
            endpoint.MapPost<DonateCommand>($"{prefix}", group)
                .Produces<Guid>()
                .WithMetadata(new SwaggerOperationAttribute("Donate for a user", "Donate for a user."));
        }
    }
}
