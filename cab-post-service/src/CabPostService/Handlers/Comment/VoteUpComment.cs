using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using CabPostService.Models.Entities;

namespace CabPostService.Handlers.Comment
{
    public partial class CommentHandler :
        ICommandHandler<VoteUpCommentCommand, bool>
    {
        public async Task<bool> Handle(
            VoteUpCommentCommand request,
            CancellationToken cancellationToken)
        {
            var postCommentRepository = _seviceProvider.GetRequiredService<IPostCommentRepository>();
            var comment = await postCommentRepository.GetOneAsync(x => x.Id == request.CommentId);
            if (comment is null)
            {
                _logger.LogError($"Cannot upvote the comment {request.CommentId}, errors: not found the post");
                throw new ApiValidationException("The comment is not found");
            }

            await postCommentRepository.UpdateAsync(comment,
                p => p.Id == comment.Id && p.CreatedAt == comment.CreatedAt,
                s => new PostComment
                {
                    UpvotesCount = comment.UpvotesCount + 1,
                    UpdatedAt = DateTime.UtcNow
                });

            comment.UpvotesCount++;

            AddVoteCommentInCache(comment, 1, 0);

            return true;
        }
    }
}
