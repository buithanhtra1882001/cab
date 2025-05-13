using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        IQueryHandler<GetPostBySlugQuery, IList<UserPostResponse>>
    {

        public async Task<IList<UserPostResponse>> Handle(
            GetPostBySlugQuery request,
            CancellationToken cancellationToken)
        {
            //var cache = _seviceProvider.GetRequiredService<IAppCache>();
            //var configuration = _seviceProvider.GetRequiredService<IConfiguration>();
            //var cacheAbsoluteExpiration = configuration.GetValue<int>("Cache:AbsoluteExpiration");
            var rng = new Random();

            //Func<Task<List<UserPostResponse>>> objectFactory = () => GetListUserPostsFactoryAsync();
            //var posts = await cache.GetOrAddAsync($"{CacheKeyConstant.POSTS}", objectFactory, DateTimeOffset.Now.AddMinutes(cacheAbsoluteExpiration));
            var posts = await GetListUserPostsFactoryAsync();
            var shufflePosts = posts.OrderBy(post => rng.Next()).ToList();
            return shufflePosts;
        }

        private async Task<List<UserPostResponse>> GetListUserPostsFactoryAsync()
        {
            var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            var filter = new GetAllPostFilter
            {
                PageNumber = 1,
                PageSize = 10000,
                IsChecked = true,
                IsSoftDeleted = false,
                Status = PostConstant.ACTIVE
            };

            var userPostViews = await postRepository.GetAllAsync(filter);
            var userIds = userPostViews.Select(item => item.UserId).Distinct();
            var users = await db.Users.Where(user => userIds.Contains(user.Id)).ToListAsync();
            var userPosts = _mapper.Map<List<UserPostResponse>>(userPostViews);

            foreach (var item in userPosts)
            {
                var user = users.FirstOrDefault(u => u.Id == item.UserId);

                if (user is null)
                    continue;

                item.UserFullName = user.Fullname;
                item.UserAvatar = user.Avatar;

                await UpdateImageHandlerAsync(item.Id);
            }

            return userPosts;
        }
    }
}