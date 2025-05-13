using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using CabPostService.Models.Entities;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        ICommandHandler<SpamPostCommand, bool>
    {
        public async Task<bool> Handle(
            SpamPostCommand request,
            CancellationToken cancellationToken)
        {
            return await Handle(request);
        }

        private async Task<bool> Handle(SpamPostCommand request)
        {
            try
            {
                var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();
                var postUserRepository = _seviceProvider.GetRequiredService<IPostUserRepository>();
                var post = await postRepository.GetByIdAsync(request.PostId);
                if (post is null)
                {
                    _logger.LogWarning($"Cannot spam the post {request.PostId}, errors: not found the post");
                    throw new ApiValidationException("The post is not found");
                }

                post.Point -= 5;

                // > 3 Dừng đề xuất trang chủ
                if (post.TotalReport > 3)
                {
                    // > 5 Dừng đề xuất all
                    if (post.TotalReport > 5)
                    {
                        post.TypeShowPost = TypeShowPostEnum.STOP_RECOMMEND_ALL;
                    }
                    else
                    {
                        post.TypeShowPost = TypeShowPostEnum.STOP_RECOMMEND_HOME;
                    }

                    InsertPostNotifyAdmin(request.PostId, post.TotalReport);
                }

                post.TotalReport += 1;
                await postRepository.UpdateAsync(post);

                var postUser = await postUserRepository.GetByUserIdAndPostId(request.UserId, request.PostId);
                if (postUser is null)
                {
                    PostUsers postUserCreate = new PostUsers()
                    {
                        PostId = request.PostId,
                        UserId = request.UserId,
                        IsShowPost = true
                    };
                    await postUserRepository.CreateAsync(postUserCreate);
                }
                else
                {
                    postUser.IsShowPost = true;
                    await postUserRepository.UpdateAsync(postUser);
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Cannot spam the post {request.PostId}, errors: {e.Message}");
                throw new ApiValidationException(e.Message);
            }

            return true;
        }

        public void InsertPostNotifyAdmin(string postId, int totalReport)
        {
            var postNotifyAdminRepository = _seviceProvider.GetRequiredService<IPostNotifyAdminRepository>();
            PostNotifyAdmin postNotifyAdmin = new PostNotifyAdmin()
            {
                Id = Guid.NewGuid().ToString(),
                PostId = postId,
                IsAcceptHide = false,
                IsHandle = false,
                IsRead = false,
                IsDelete = false,
                ReportNumber = totalReport,
                Description = "Report post",
                CreatedAt = DateTime.Now
            };
            postNotifyAdminRepository.InsertOne(postNotifyAdmin);
        }
    }
}