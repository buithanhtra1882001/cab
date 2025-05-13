using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        IQueryHandler<GetLatestVideosQuery, PagingResponse<GetAllPostResponse>?>
    {
        public async Task<PagingResponse<GetAllPostResponse>?> Handle(GetLatestVideosQuery request, CancellationToken cancellationToken)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            var response = new PagingResponse<GetAllPostResponse>();

            var query = db.Posts
                .AsNoTracking()
                .Where(x => !x.IsSoftDeleted
                            && x.IsChecked
                            && x.Status == PostConstant.ACTIVE
                            && x.PostType == "video")
                .OrderByDescending(x => x.CreatedAt);

            var latestVideos = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var result = _mapper.Map<List<GetAllPostResponse>>(latestVideos);
            if (!result.Any())
                return response;

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
