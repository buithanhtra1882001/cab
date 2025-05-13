using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using LazyCache;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.Comment
{
    public partial class CommentHandler :
        ICommandHandler<ReplyCommand, Guid>
    {
        public async Task<Guid> Handle(
            ReplyCommand request,
            CancellationToken cancellationToken)
        {
            var comment = await ValidateCreateReplyAndReturnCommentAsync(request);

            var postCommentReplyRepository = _seviceProvider.GetRequiredService<IPostCommentReplyRepository>();

            var reply = new PostCommentReply()
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Content = request.Content,
                CommentId = request.ReplyToCommentId,
                Status = PostConstant.ACTIVE
            };

            var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();

            var post = await postRepository.GetByIdAsync(comment.PostId);
            await postCommentReplyRepository.CreateAsync(reply);
            await postRepository.UpdateCommentCount(post.Id);

            UpdatePostFromCache(post.Id);

            var mediator = _seviceProvider.GetRequiredService<IMediator>();
            var userBehaviorRequest = new UserBehaviorRequest
            {
                UserId = request.UserId,
                PostId = post.Id,
                Type = UserActionType.Reply
            };

            await mediator.Send(userBehaviorRequest);

            if (reply is not null && !string.IsNullOrEmpty(reply.Content))
                await AddReplyToCache(reply);

            return reply is not null ? reply.Id : new Guid();
        }

        private async Task<PostComment> ValidateCreateReplyAndReturnCommentAsync(ReplyCommand replyRequest)
        {
            var postCommentRepository = _seviceProvider.GetRequiredService<IPostCommentRepository>();

            if (string.IsNullOrEmpty(replyRequest.Content))
                throw new ApiValidationException("Content is null or empty");

            var comment = await postCommentRepository.GetOneAsync(x => x.Id == replyRequest.ReplyToCommentId);
            if (comment is null)
            {
                _logger.LogError($"Cannot reply to the reply, errors: not found the comment");
                throw new ApiValidationException("The comment is not found");
            }

            var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();
            var post = await postRepository.GetByIdAsync(comment.PostId);
            if (post is null)
            {
                _logger.LogWarning($"Not found post with postId={comment.PostId}");
                throw new ApiValidationException("The post is not found");
            }

            return comment;
        }

        private async Task AddReplyToCache(PostCommentReply reply)
        {
            var cache = _seviceProvider.GetRequiredService<IAppCache>();
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
            var newReply = _mapper.Map<UserReplyResponse>(reply);
            var replies = cache.Get<List<UserReplyResponse>>($"{CacheKeyConstant.POST_COMMENT_REPLIES}_{reply.CommentId}");
            var user = await db.Users.FirstOrDefaultAsync(item => item.Id == newReply.UserId);

            newReply.UserFullName = user is null ? string.Empty : user.Fullname;
            newReply.Avatar = user is null ? string.Empty : user.Avatar;
            replies?.Add(newReply);
        }
    }
}
