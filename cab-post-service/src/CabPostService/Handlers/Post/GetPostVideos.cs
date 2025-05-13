using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using MediatR;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        IQueryHandler<GetPostVideosQuery, PagingResponse<GetAllPostResponse>>
    {
        public async Task<PagingResponse<GetAllPostResponse>> Handle(GetPostVideosQuery request, CancellationToken cancellationToken)
        {
            var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();
            var mediator = _seviceProvider.GetRequiredService<IMediator>();

            var response = new PagingResponse<GetAllPostResponse>();

            var pagingRequest = _mapper.Map<GetPostVideosQuery, GetPostVideosOrderByCreatedWithPaging>(request);
            var result = await postRepository.GetPostVideosAsync(pagingRequest);

            if (!result.Any())
                return response;

            result = await mediator.Send(new PopulatePostDetailsQuery
            {
                Posts = result,
                UserId = request.UserId
            });

            var requestMore = new GetPostVideosQuery
            {
                PageNumber = request.PageNumber,
                PageSize = 1,
                UserId = request.UserId
            };
            var pagingRequestMore = _mapper.Map<GetPostVideosQuery, GetPostVideosOrderByCreatedWithPaging>(requestMore);
            var resultMore = await postRepository.GetPostVideosAsync(pagingRequestMore);

            response.HasMore = resultMore.Count() > 0;
            response.Data = result;
            response.PageSize = request.PageSize;
            response.PageNumber = request.PageNumber;
            response.PagingState = string.Empty;
            response.Total = 0;

            return response;
        }
    }
}
