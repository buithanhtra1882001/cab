using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
       IQueryHandler<GetVideoSuggestQuery, PagingResponse<GetAllPostResponse>?>
    {
        public async Task<PagingResponse<GetAllPostResponse>?> Handle(GetVideoSuggestQuery request, CancellationToken cancellationToken)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
            var response = new PagingResponse<GetAllPostResponse>();

            var postEntity = await db.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.PostId);
            //if (postEntity is null)
            //    throw new AppException($"Post not found with postId = {request.PostId}");

            var videoSuggestions = await GetVideoSuggestionsAsync(postEntity, request);

            if (!videoSuggestions.Any())
                return response;

            var result = _mapper.Map<List<GetAllPostResponse>>(videoSuggestions);
            await FetchAdditionalDataForPostResponses(request.UserId, result);

            response.HasMore = result.Count == request.PageSize;
            response.Data = result;
            response.PageSize = request.PageSize;
            response.PageNumber = request.PageNumber;
            response.PagingState = string.Empty;
            response.Total = result.Count;

            return response;
        }

        private async Task FetchAdditionalDataForPostResponses(Guid userId, List<GetAllPostResponse> result)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
            var mediator = _seviceProvider.GetRequiredService<IMediator>();

            result = await mediator.Send(new PopulatePostDetailsQuery
            {
                Posts = result,
                UserId = userId,
            });

            var userIds = result.Select(x => x.UserId).ToList();
            var userEntities = await db.Users
                .AsNoTracking()
                .Where(x => userIds.Contains(x.Id)).ToListAsync();

            foreach (var postVideo in result)
            {
                var userByPostId = userEntities.FirstOrDefault(x => x.Id == postVideo.UserId);

                postVideo.UserFullName = userByPostId?.Fullname ?? string.Empty;
                postVideo.UserAvatar = userByPostId?.Avatar ?? string.Empty;
            }
        }

        private async Task<List<Models.Entities.Post>> GetVideoSuggestionsAsync
            (Models.Entities.Post post, GetVideoSuggestQuery request)
        {
            var videoSuggestions = new List<Models.Entities.Post>();

            var skipAmount = (request.PageNumber - 1) * request.PageSize;
            var pageSizeRemaining = request.PageSize;

            videoSuggestions.AddRange(await GetVideoByHashtagAndCategoryAsync(post, skipAmount, pageSizeRemaining));
            pageSizeRemaining -= videoSuggestions.Count;

            if (pageSizeRemaining > 0)
            {
                skipAmount = videoSuggestions.Count;
                videoSuggestions.AddRange(await GetTrendingVideosAsync(skipAmount, pageSizeRemaining));

                pageSizeRemaining -= videoSuggestions.Count;
            }
            if (pageSizeRemaining > 0)
            {
                skipAmount = videoSuggestions.Count;
                videoSuggestions.AddRange(await GetRandomCategoryVideosAsync(skipAmount, pageSizeRemaining));
            }

            return videoSuggestions.Distinct().ToList() ?? new List<Models.Entities.Post>();
        }

        private async Task<List<Models.Entities.Post>> GetVideoByHashtagAndCategoryAsync
            (Models.Entities.Post post, int skipAmount, int takeAmount)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
            //var hashagsFromPost = post.Hashtags?.Split(",").ToList() ?? new List<string>();
            //var pattern = $"%{string.Join("%", hashagsFromPost)}%";

            return post is null ? new List<Models.Entities.Post>() : await db.Posts
                .AsNoTracking()
                .Where(x => !x.IsSoftDeleted
                            && x.IsChecked
                            && x.Status == PostConstant.ACTIVE
                            && x.PostType == "video"
                            && x.CategoryId == post.CategoryId
                            && x.IsPersonalPost == true
                            /*&& (string.IsNullOrEmpty(post.Hashtags) || EF.Functions.Like(x.Hashtags, pattern))*/)
                .Skip(skipAmount)
                .Take(takeAmount)
                .ToListAsync();
        }
        private async Task<List<Models.Entities.Post>> GetTrendingVideosAsync
            (int skipAmount, int takeAmount)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
            return await db.Posts
                .AsNoTracking()
                .Where(x => !x.IsSoftDeleted
                            && x.IsChecked
                            && x.Status == PostConstant.ACTIVE
                            && x.PostType == "video"
                            && x.IsTopTrending && x.IsPersonalPost == true)
                .Skip(skipAmount)
                .Take(takeAmount)
                .ToListAsync();
        }
        private async Task<List<Models.Entities.Post>> GetRandomCategoryVideosAsync
            (int skipAmount, int takeAmount)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
            return await db.Posts
                .AsNoTracking()
                .Where(x => !x.IsSoftDeleted
                            && x.IsChecked
                            && x.Status == PostConstant.ACTIVE
                            && x.PostType == "video" && x.IsPersonalPost == true)
                .OrderBy(x => Guid.NewGuid())
                .Skip(skipAmount)
                .Take(takeAmount)
                .ToListAsync();
        }
    }
}
