using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using MediatR;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        IQueryHandler<GetAllPostOrderByPointWithPagingQuery, PagingResponse<GetAllPostResponse>>
    {
        public async Task<PagingResponse<GetAllPostResponse>> Handle(
            GetAllPostOrderByPointWithPagingQuery request,
            CancellationToken cancellationToken)
        {
            var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();
            var mediator = _seviceProvider.GetRequiredService<IMediator>();

            var response = new PagingResponse<GetAllPostResponse>();

            var pagingRequest = _mapper.Map<GetAllPostOrderByPointWithPagingQuery, GetAllPostOrderByPointWithPaging>(request);
            var result = await postRepository.GetOrderedPostsAsync(pagingRequest);

            if (!result.Any())
                return response;

            result = await mediator.Send(new PopulatePostDetailsQuery
            {
                Posts = result,
                UserId = request.UserId
            });

            var requestMore = new GetAllPostOrderByPointWithPagingQuery
            {
                PageNumber = request.PageNumber,
                PageSize = 1,
                UserId = request.UserId
            };
            var pagingRequestMore = _mapper.Map<GetAllPostOrderByPointWithPagingQuery, GetAllPostOrderByPointWithPaging>(requestMore);
            var resultMore = await postRepository.GetOrderedPostsAsync(pagingRequestMore);

            response.HasMore = resultMore.Count() > 0;
            response.Data = result;
            response.PageSize = request.PageSize;
            response.PageNumber = request.PageNumber;
            response.PagingState = string.Empty;
            response.Total = 0;

            return response;
        }

        private async Task<PagingResponse<UserCommentResponse>> GetCommentsByPostIdAsync(string postId, int pageSize)
        {
            var mediator = _seviceProvider.GetRequiredService<IMediator>();
            var commentRequest = new GetCommentQuery
            {
                PostId = postId,
                PageSize = pageSize
            };
            var postComments = await mediator.Send(commentRequest);
            foreach (var postComment in postComments.Data)
            {
                var replyRequest = new GetReplyQuery
                {
                    CommentId = postComment.Id,
                    PageSize = pageSize
                };
                postComment.Replies = await mediator.Send(replyRequest);
            }
            return postComments;
        }
    }
}
