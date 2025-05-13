using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using LazyCache;

namespace CabPostService.Handlers.Comment
{
    public partial class CommentHandler :
        ICommandHandler<VoteDownCommentCommand, bool>
    {
        public async Task<bool> Handle(
            VoteDownCommentCommand request,
            CancellationToken cancellationToken)
        {
            var postCommentRepository = _seviceProvider.GetRequiredService<IPostCommentRepository>();
            var comment = await postCommentRepository.GetOneAsync(x => x.Id == request.CommentId);
            if (comment is null)
            {
                _logger.LogError($"Cannot downvote the comment {request.CommentId}, errors: not found the post");
                throw new ApiValidationException("The comment is not found");
            }

            await postCommentRepository.UpdateAsync(comment,
                            p => p.Id == comment.Id && p.CreatedAt == comment.CreatedAt,
                            s => new PostComment
                            {
                                DownvotesCount = comment.DownvotesCount + 1,
                                UpdatedAt = DateTime.UtcNow
                            });

            comment.DownvotesCount++;

            AddVoteCommentInCache(comment, 0, 1);

            return true;
        }

        private void AddVoteCommentInCache(PostComment comment, int upvote, int downvote)
        {
            var cache = _seviceProvider.GetRequiredService<IAppCache>();

            var comments = cache.Get<List<UserCommentResponse>>($"{CacheKeyConstant.POST_COMMENTS}_{comment.PostId}");
            if (comments is null)
                return;

            var updatedComment = comments.FirstOrDefault(item => item.Id == comment.Id);
            if (updatedComment is not null)
            {
                updatedComment.DownvotesCount += downvote;
                updatedComment.UpvotesCount += upvote;
            }
        }
    }
}
