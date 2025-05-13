using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        IQueryHandler<GetTopTrendingQuery, PagingResponse<GetAllPostResponse>?>
    {
        public async Task<PagingResponse<GetAllPostResponse>?> Handle(GetTopTrendingQuery request, CancellationToken cancellationToken)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
            var mediator = _seviceProvider.GetRequiredService<IMediator>();

            var response = new PagingResponse<GetAllPostResponse>();

            var query = db.Posts
                .AsNoTracking()
                .Where(x => !x.IsSoftDeleted
                            && x.IsChecked
                            && x.Status == PostConstant.ACTIVE
                            && request.PostType == "video" ? x.PostType == "video" : x.PostType != "video")
                .OrderByDescending(x => x.TrendingPoint);

            var postsTopTrending = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var result = _mapper.Map<List<GetAllPostResponse>>(postsTopTrending);

            if (!result.Any())
                return response;

            result = await mediator.Send(new PopulatePostDetailsQuery
            {
                Posts = result,
                UserId = request.UserId,
            });

            var resultMore = await query
                .Skip(request.PageNumber * request.PageSize)
                .Take(1)
                .ToListAsync();

            response.HasMore = resultMore.Count() > 0;
            response.Data = result;
            response.PageSize = request.PageSize;
            response.PageNumber = request.PageNumber;
            response.PagingState = string.Empty;
            response.Total = 0;

            return response;
        }
    }
}
