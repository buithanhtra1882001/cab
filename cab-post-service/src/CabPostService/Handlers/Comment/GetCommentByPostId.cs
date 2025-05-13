using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using LazyCache;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.Comment
{
    public partial class CommentHandler : IQueryHandler<GetCommentQuery, PagingResponse<UserCommentResponse>>
    {
        public async Task<PagingResponse<UserCommentResponse>> Handle(GetCommentQuery request, CancellationToken cancellationToken)
        {
            var cache = _seviceProvider.GetRequiredService<IAppCache>();
            var configuration = _seviceProvider.GetRequiredService<IConfiguration>();
            var cacheAbsoluteExpiration = configuration.GetValue<int>("Cache:AbsoluteExpiration");

            //Func<Task<PagingResponse<UserCommentResponse>>> objectFactory = () => GetPostCommentsFactory(request);

            //var result = (await cache.GetOrAddAsync(
            //    $"{CacheKeyConstant.POST_COMMENTS}_{request.PostId}_{request.PageSize}_{request.PagingState}",
            //    objectFactory,
            //    DateTimeOffset.Now.AddMinutes(cacheAbsoluteExpiration)));
            var result = await GetPostCommentsFactory(request);

            return result;
        }

        private async Task<PagingResponse<UserCommentResponse>> GetPostCommentsFactory(GetCommentQuery request)
        {
            var response = new PagingResponse<UserCommentResponse>();
            var postCommentPostIdMaterializedViewRepository = _seviceProvider
                .GetRequiredService<IPostCommentPostIdMaterializedViewRepository>();
            var commentLikeRepository = _seviceProvider.GetRequiredService<ICommentLikeRepository>();

            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            var postCommentReplyCommentIdMaterializedViewRepository = _seviceProvider
                .GetRequiredService<IPostCommentReplyCommentIdMaterializedViewRepository>();

            byte[] pagingStateByte = string.IsNullOrEmpty(request.PagingState) ? null : Convert.FromBase64String(request.PagingState);

            var (data, pagingState) = await postCommentPostIdMaterializedViewRepository.GetListPagingAsync(x => x.PostId == request.PostId, request.PageSize, pagingStateByte);

            if (!data.Any())
            {
                return response;
            }

            var userComments = _mapper.Map<List<UserCommentResponse>>(data);
            var userCommentIds = userComments.Select(userComment => userComment.Id).Distinct();
            var userIds = userComments.Select(item => item.UserId).Distinct();
            var users = await db.Users.Where(user => userIds.Contains(user.Id)).ToListAsync();
            var currentInteractedComments = await commentLikeRepository.GetListAsync
                (x => x.UserId == request.UserId && userCommentIds.Contains(x.CommentId));
            var lstCurrentInteractedComments = currentInteractedComments.ToList();
            //var commentReplies = (await postCommentReplyCommentIdMaterializedViewRepository
            //    .GetListAsync(reply => userCommentIds
            //    .Contains(reply.CommentId)))
            //    .ToList();

            foreach (var comment in userComments)
            {
                var user = users.FirstOrDefault(u => u.Id == comment.UserId);
                // var reply = commentReplies.FindAll(commentReply => commentReply.CommentId == comment.Id);
                comment.UserFullName = user?.Fullname ?? "";
                comment.Avatar = user?.Avatar ?? "";
                //comment.Replies = _mapper.Map<List<UserReplyResponse>>(reply);
                var interactedComment = lstCurrentInteractedComments.FirstOrDefault(x => x.CommentId == comment.Id);
                comment.CurrentUserHasLike = interactedComment?.LikeType == LikeType.Like;
                comment.CurrentUserHasUnlike = interactedComment?.LikeType == LikeType.Unlike;
                comment.TotalReply = await postCommentReplyCommentIdMaterializedViewRepository.CountAsync(x => x.CommentId == comment.Id);
            }

            response.Data = userComments;
            response.PagingState = pagingState;
            response.PageSize = request.PageSize;
            response.PageNumber = request.PageNumber;
            response.Total = await postCommentPostIdMaterializedViewRepository.CountAsync(x => x.PostId == request.PostId);
            response.HasMore = response.Total - (response.PageNumber * request.PageSize) > 0;

            return response;
        }
    }
}
