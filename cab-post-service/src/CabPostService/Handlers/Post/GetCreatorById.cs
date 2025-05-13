using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        IQueryHandler<GetCreatorByIdQuery, IList<CreatorResponse>>
    {
        public async Task<IList<CreatorResponse>> Handle(
            GetCreatorByIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (request is null)
                    return new List<CreatorResponse>();

                var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
                var postIds = await GetPostIdsByCreatorsAsync(request.CreatorIds);
                var userVoteIds = await GetUserVoteIdsAsync(request.UserId, postIds);
                //var userCommentIds = await GetUserCommentIdsAsync(request.UserId, postIds);
                //if (!userVoteIds.Any() && !userCommentIds.Any())
                //{
                //    return new List<CreatorResponse>();
                //}
                //var creatorIds = userCommentIds.Concat(userVoteIds).Distinct().ToList();
                var creatorIds = userVoteIds.Distinct().ToList();
                var creators = await db.Users.Where(user => creatorIds.Contains(user.Id)).ToListAsync();
                return _mapper.Map<List<CreatorResponse>>(creators);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in PostHandler");
                throw;

            }
        }

        private async Task<List<string>> GetPostIdsByCreatorsAsync(List<Guid> creatorIds)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
            return await db.Posts.AsNoTracking().Where(x => creatorIds.Contains(x.UserId)).Take(50)
                                       .Select(x => x.Id)
                                       .ToListAsync();
        }
        private async Task<List<Guid>> GetUserVoteIdsAsync(Guid userId, List<string> postIds)
        {
            if (postIds == null || !postIds.Any())
                return new List<Guid>();

            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
            return await db.PostVotes.AsNoTracking().Where(x => postIds.Contains(x.PostId) && x.UserVoteId == userId)
                                     .Include(x => x.Post)
                                     .Select(x => x.Post.UserId).Distinct().ToListAsync();
        }

        private async Task<List<Guid>> GetUserCommentIdsAsync(Guid userId, List<string> postIds)
        {
            if (postIds == null || !postIds.Any())
                return new List<Guid>();

            var postCommentRepository = _seviceProvider.GetRequiredService<IPostCommentRepository>();
            var userCommentIds = new List<Guid>();

            foreach (var postId in postIds)
            {
                var userComment = await postCommentRepository.GetOneAsync(x => x.PostId == postId && x.UserId == userId);
                if (userComment is not null)
                    userCommentIds.Add(userComment.UserId);

            }
            return userCommentIds.Distinct().ToList();

            //var userComment = await postCommentRepository.GetListAsync(x => postIds.Contains(x.PostId) && x.UserId == userId);
            //return userComment.Select(x => x.UserId).Distinct().ToList();
            //(IN predicates on non-primary-key columns (post_id) is not yet supported)
        }
    }
}
