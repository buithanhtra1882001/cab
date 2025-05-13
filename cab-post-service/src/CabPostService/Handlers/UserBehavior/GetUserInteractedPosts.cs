using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.UserBehavior
{
    public partial class UserBehaviorHandler :
        IQueryHandler<GetUserInteractedPostsQuery, IList<PostInteractionResponse>>
    {
        public async Task<IList<PostInteractionResponse>> Handle(
            GetUserInteractedPostsQuery request,
            CancellationToken cancellationToken)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            var user = await db.Users.FindAsync(request.UserId);
            if (user is null)
            {
                _logger.LogWarning($"Not found user with userId={request.UserId}");
                throw new ApiValidationException("The user is not found");
            }

            var userBehaviorList = await db.UserBehaviors.AsNoTracking()
                                                         .Where(x => x.UserId == request.UserId &&
                                                                x.IsHidden == false)
                                                         .OrderByDescending(x => x.CreatedAt)
                                                         .ToListAsync();

            if (!userBehaviorList.Any()) return new List<PostInteractionResponse>();

            var postIds = userBehaviorList.Select(x => x.PostId).ToList();
            var postEntity = await db.Posts.Where(x => postIds.Contains(x.Id)).ToListAsync();
            var userPosts = _mapper.Map<List<UserPostResponse>>(postEntity);
            foreach (var userPost in userPosts)
            {
                userPost.UserFullName = user is null ? "" : user.Fullname;
                userPost.UserAvatar = user is null ? "" : user.Avatar;
            }
            var userBehaviorResponses = userBehaviorList.Select(x => new PostInteractionResponse
            {
                Type = x.Type,
                UserPostResponse = userPosts.FirstOrDefault(p => p.Id == x.PostId) ?? new UserPostResponse()
            }).ToList();

            return userBehaviorResponses;
        }
    }
}
