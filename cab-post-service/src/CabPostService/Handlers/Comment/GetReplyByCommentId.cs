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
    public partial class CommentHandler :
        IQueryHandler<GetReplyQuery, PagingResponse<UserReplyResponse>>
    {
        public async Task<PagingResponse<UserReplyResponse>> Handle(
            GetReplyQuery request,
            CancellationToken cancellationToken)
        {
            var cache = _seviceProvider.GetRequiredService<IAppCache>();
            var configuration = _seviceProvider.GetRequiredService<IConfiguration>();
            var cacheAbsoluteExpiration = configuration.GetValue<int>("Cache:AbsoluteExpiration");

            //Func<Task<PagingResponse<UserReplyResponse>>> objectFactory = () => GetPostCommentRepliesFactory(request);

            //var result = (await cache.GetOrAddAsync(
            //    $"{CacheKeyConstant.POST_COMMENT_REPLIES}_{request.CommentId}_{request.PageSize}_{request.PagingState}",
            //    objectFactory,
            //    DateTimeOffset.Now.AddMinutes(cacheAbsoluteExpiration)));

            var result = await GetPostCommentRepliesFactory(request);

            return result;
        }

        private async Task<PagingResponse<UserReplyResponse>> GetPostCommentRepliesFactory(GetReplyQuery request)
        {
            var response = new PagingResponse<UserReplyResponse>();
            var postCommentReplyCommentIdMaterializedViewRepository = _seviceProvider
               .GetRequiredService<IPostCommentReplyCommentIdMaterializedViewRepository>();

            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            byte[] pagingStateByte = string.IsNullOrEmpty(request.PagingState) ? null : Convert.FromBase64String(request.PagingState);

            var (data, pagingState) = await postCommentReplyCommentIdMaterializedViewRepository.GetListPagingAsync(x => x.CommentId == request.CommentId, request.PageSize, pagingStateByte);
            if (!data.Any()) return response;

            var replies = _mapper.Map<List<UserReplyResponse>>(data);
            var userIds = replies.Select(u => u.UserId).ToList();
            var users = await db.Users.Where(user => userIds.Contains(user.Id)).ToListAsync();

            foreach (var item in replies)
            {
                var user = users.FirstOrDefault(u => u.Id == item.UserId);
                if (user is null)
                    continue;
                //var reply = replies.FindAll(r => r.CommentId == item.Id);
                item.UserFullName = user?.Fullname ?? "";
                item.Avatar = user?.Avatar ?? "";
            }

            response.Data = replies;
            response.PagingState = pagingState;
            response.PageSize = request.PageSize;
            response.PageNumber = request.PageNumber;
            response.Total = await postCommentReplyCommentIdMaterializedViewRepository.CountAsync(x => x.CommentId == request.CommentId);
            response.HasMore = response.Total - (response.PageNumber * request.PageSize) > 0;

            return response;
        }
    }
}
