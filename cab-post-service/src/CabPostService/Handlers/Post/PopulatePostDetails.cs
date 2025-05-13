using CabPostService.Constants;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        IRequestHandler<PopulatePostDetailsQuery, List<GetAllPostResponse>>
    {
        public async Task<List<GetAllPostResponse>> Handle(PopulatePostDetailsQuery request, CancellationToken cancellationToken)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
            var postImageRepository = _seviceProvider.GetRequiredService<IPostImageRepository>();

            var result = request.Posts;

            var postIds = result.Select(x => x.Id).ToList();

             var postVoteEntities = await db.PostVotes
            .AsNoTracking()
                .Where(x => x.UserVoteId == request.UserId && postIds.Contains(x.PostId))
                .ToListAsync();

            var postImageEntities = await postImageRepository.GetPostImagesAsync(postIds);

            var postVideoEntities = await db.PostVideos
                .AsNoTracking()
                .Where(x => postIds.Contains(x.PostId))
                .ToListAsync();

            foreach (var item in result)
            {
                //item.UserCommentResponses = await GetCommentsByPostIdAsync(item.Id, request.PageSize);
                var postImagesByPostId = postImageEntities.FindAll(x => x.PostId == item.Id);
                //if (!postImagesByPostId.Any())
                //    continue;

                item.PostImageResponses = postImagesByPostId?.Select(postImage => new PostImageResponse
                {
                    ImageId = postImage.ImageId,
                    Url = postImage.Url,
                    IsViolence = postImage.IsViolence
                }).ToList() ?? new List<PostImageResponse>();

                var postVideoByPostId = postVideoEntities.FindAll(x => x.PostId == item.Id);

                item.PostVideoResponses = postVideoByPostId?.Select(postVideo => new PostVideoResponse
                {
                    VideoId = postVideo.MediaVideoId,
                    Url = postVideo.VideoUrl,
                    IsViolence = postVideo.IsViolence
                }).ToList() ?? new List<PostVideoResponse>();

                var postVoteEntity = postVoteEntities.FirstOrDefault(x => x.PostId == item.Id);
                if (postVoteEntity is not null)
                {
                    item.CurrentUserHasVoteUp = postVoteEntity.Type == PostConstant.VOTE_UP;
                    item.CurrentUserHasVoteDown = postVoteEntity.Type == PostConstant.VOTE_DOWN;
                }
            }
            return result;
        }
    }
}
