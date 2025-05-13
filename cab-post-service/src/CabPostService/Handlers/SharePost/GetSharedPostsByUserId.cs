using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using Polly.Caching;

namespace CabPostService.Handlers.SharePost
{
    public partial class SharePostHandler :
    IQueryHandler<GetSharePostByUserIdQuery, PagingResponse<SharePostResponse>>
    {
        public async Task<PagingResponse<SharePostResponse>> Handle(
          GetSharePostByUserIdQuery request,
          CancellationToken cancellationToken)
        {
            var postShareFilter = _mapper.Map<SharePostFilter>(request);

            var sharePosts = await GetPostShareByUserIdFromDatabase(postShareFilter);

            var total = await SharePostRepository.GetTotalAsync(postShareFilter);

            foreach (var sharePost in sharePosts)
            {
                await AddPostShareToCache(sharePost);
            }

            return new PagingResponse<SharePostResponse>
            {
                Data = sharePosts.ToList(),
                Total = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                PagingState = string.Empty,
                HasMore = total - (request.PageNumber * request.PageSize) > 0
            };
        }

        private async Task<IEnumerable<SharePostResponse>> GetPostShareByUserIdFromDatabase(SharePostFilter sharePostFilter)
        {
            var sharePosts = await SharePostRepository.GetAllPostShareByFilterAsync(sharePostFilter);

            return _mapper.Map<IEnumerable<SharePostResponse>>(sharePosts);
        }
    }
}
