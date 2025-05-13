using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;

namespace CabPostService.Handlers.SharePost
{
    public partial class SharePostHandler :
        IQueryHandler<GetSharePostByIdQuery, SharePostResponse>
    {
        public async Task<SharePostResponse> Handle(
            GetSharePostByIdQuery request,
            CancellationToken cancellationToken)
        {
            var postShareCache = await AppCache.GetAsync<List<SharePostResponse>>(CacheKeyConstant.POST_SHARES);

            var postShareInCache = postShareCache?.FirstOrDefault(item => item.Id == request.Id);

            if (postShareInCache is not null)
                return postShareInCache;

            var postShare = await SharePostRepository.GetByIdAsync(request.Id);

            var postShareResponse = _mapper.Map<SharePostResponse>(postShare);

            await AddPostShareToCache(postShareResponse);

            return postShareResponse;
        }
    }
}
