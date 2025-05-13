using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        IQueryHandler<GetAllPostFilterCommand, PagingResponse<AdminGetPostsResponse>>
    {
        public async Task<PagingResponse<AdminGetPostsResponse>> Handle(
            GetAllPostFilterCommand request,
            CancellationToken cancellationToken)
        {
            var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            var filter = _mapper.Map<GetAllPostFilterCommand, GetAllPostFilter>(request);

            var posts = await postRepository.GetAllAsync(filter);
            var total = await postRepository.GetTotalAsync(filter);
            var userIds = posts.Select(item => item.UserId);
            var users = await db.Users.Where(user => userIds.Contains(user.Id)).ToListAsync();
            var data = _mapper.Map<List<AdminGetPostsResponse>>(posts);

            foreach (var item in data)
            {
                var user = users.FirstOrDefault(u => u.Id == item.UserId);

                if (user is null)
                    continue;

                item.UserFullName = user.Fullname;
                item.UserAvatar = user.Avatar;
            }

            return new PagingResponse<AdminGetPostsResponse>
            {
                Data = data,
                Total = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                HasMore = data.Count() == request.PageSize,
                PagingState = string.Empty
            };
        }
    }
}