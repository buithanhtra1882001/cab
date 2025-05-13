using CabPostService.Cqrs.Requests.Commands;
using CabPostService.Cqrs.Requests.Queries;
using CabPostService.Models.Dtos;

namespace CabPostService.Services.Abstractions
{
    public interface IUserService
    {
        Task<StatisticalUserDto> GetStatisticalUserAsync(StatisticalUserQuery request, CancellationToken cancellationToken);
    }
}
