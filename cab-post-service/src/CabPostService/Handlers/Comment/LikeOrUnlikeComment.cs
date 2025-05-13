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
        ICommandHandler<LikeOrUnlikeCommentCommand, bool>
    {
        public async Task<bool> Handle(
            LikeOrUnlikeCommentCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var postCommentRepository = _seviceProvider.GetRequiredService<IPostCommentRepository>();
                var commentLikeRepository = _seviceProvider.GetRequiredService<ICommentLikeRepository>();

                var comment = await postCommentRepository.GetOneAsync(x => x.Id == request.CommentId);
                if (comment == null)
                {
                    _logger.LogError($"Cannot process the comment {request.CommentId}, error: not found");
                    throw new ApiValidationException("The comment is not found");
                }

                var commentLikeEntity = new CommentLike
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    CommentId = request.CommentId,
                    LikeType = request.Type,
                };

                await commentLikeRepository.CreateAsync(commentLikeEntity);

                if (request.Type == LikeType.Like)
                {
                    comment.UpvotesCount++;
                    await postCommentRepository.UpdateAsync(comment,
                       p => p.Id == comment.Id && p.CreatedAt == comment.CreatedAt,
                       s => new PostComment
                       {
                           UpvotesCount = comment.UpvotesCount,
                           UpdatedAt = DateTime.UtcNow
                       });
                }
                else if (request.Type == LikeType.Unlike)
                {
                    comment.DownvotesCount++;
                    await postCommentRepository.UpdateAsync(comment,
                       p => p.Id == comment.Id && p.CreatedAt == comment.CreatedAt,
                       s => new PostComment
                       {
                           DownvotesCount = comment.DownvotesCount,
                           UpdatedAt = DateTime.UtcNow
                       });
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing the comment {request.CommentId}");
                throw;
            }
        }
    }
}
