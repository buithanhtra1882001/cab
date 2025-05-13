using CabPostService.Clients.Interfaces;
using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using LazyCache;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        IQueryHandler<GetPostByIdQuery, GetAllPostResponse?>
    {
        public async Task<GetAllPostResponse?> Handle(
            GetPostByIdQuery request,
            CancellationToken cancellationToken)
        {

            if (Guid.TryParse(request.PostId, out Guid postIdGuid))
            {
                request.PostId = postIdGuid.ToString();
            }

            var cache = _seviceProvider.GetRequiredService<IAppCache>();
            var mediator = _seviceProvider.GetRequiredService<IMediator>();

            var postsCache = await cache.GetAsync<List<GetAllPostResponse>>(CacheKeyConstant.POSTS);

            var postInCache = postsCache is not null && postsCache.Any(item => item.Id == request.PostId);

            if (postInCache)
            {
                await UpdateImageHandlerAsync(request.PostId);

                if (postsCache is null)
                    return null;

                return postsCache.FirstOrDefault(item => item.Id == request.PostId);
            }
            else
            {
                var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();
                var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

                var postInDb = await postRepository.GetByIdAsync(request.PostId);

                if (postInDb is null)
                    return null;

                var postIsValidToUpdate = !postInDb.IsSoftDeleted &&
                                          postInDb.IsChecked &&
                                          postInDb.Status is PostConstant.ACTIVE;
                if (!postIsValidToUpdate)
                    return null;

                var data = _mapper.Map<GetAllPostResponse>(postInDb);

                var user = await db.Users.FirstOrDefaultAsync(item => item.Id == postInDb.UserId);
                if (user is null)
                    return null;

                data.UserFullName = user is null ? "" : user.Fullname;
                data.UserAvatar = user is null ? "" : user.Avatar;

                var result = await mediator.Send(new PopulatePostDetailsQuery
                {
                    Posts = new List<GetAllPostResponse> { data },
                    UserId = postInDb.UserId
                });
                
                //await UpdateImageHandlerAsync(request.PostId);

                return result.FirstOrDefault();
            }
        }

        private async Task UpdateImageHandlerAsync(string postId)
        {
            var postImageRepository = _seviceProvider.GetRequiredService<IPostImageRepository>();

            var postImageEntity = await postImageRepository.GetOneAsync(x => x.PostId == postId);

            if (postImageEntity is null)
                return;

            var mediaClient = _seviceProvider.GetRequiredService<IMediaClient>();

            await mediaClient.UpdateImagesAsync(BearerToken, new List<Guid> { postImageEntity.ImageId });
        }
    }
}