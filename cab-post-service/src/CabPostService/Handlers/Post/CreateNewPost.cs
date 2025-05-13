using CAB.BuildingBlocks.EventBus.Abstractions;
using CabPostService.Common.Providers;
using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Helpers;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.IntegrationEvents.Events;
using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Minio.DataModel;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.XPath;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        ICommandHandler<CreatePostCommand, GetAllPostResponse>
    {
        public async Task<GetAllPostResponse> Handle(
            CreatePostCommand request,
            CancellationToken cancellationToken)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            await ValidationCreatePostAsync(request);

            var post = _mapper.Map<Models.Entities.Post>(request);
            post.Id = Guid.NewGuid().ToString();
            post.IsChecked = true;
            post.Status = PostConstant.ACTIVE;

            var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();
            var postImageRepository = _seviceProvider.GetRequiredService<IPostImageRepository>();
            var postVideoRepository = _seviceProvider.GetRequiredService<IPostVideoRepository>();

            await postRepository.CreateAsync(post);

            if (request.ImageInfo.Any())
            {
                await postImageRepository.CreateManyAsync(request.ImageInfo.Select(x => new PostImage
                {
                    Id = Guid.NewGuid(),
                    PostId = post.Id,
                    ImageId = x.ImageId,
                    Url = x.ImageUrl,
                    IsViolence = false
                }));
            }

            if (request.VideoInfo.Any())
            {
                db.PostVideos.AddRange(request.VideoInfo.Select(x => new Models.Entities.PostVideo
                {
                    Id = Guid.NewGuid().ToString(),
                    MediaVideoId = x.VideoId,
                    PostId = post.Id,
                    VideoUrl = x.VideoUrl,
                    IsViolence = false,
                    UserId = post.UserId,
                    AvgViewLength = 0,
                    Description = string.Empty,
                    LengthVideo = 0,
                    ViewCount = 0,

                }));

                await db.SaveChangesAsync();
            }

            await AddOrUpdateHashtag(request.Hashtags);
            await AddPostToCache(post);

            var result = _mapper.Map<GetAllPostResponse>(post);

            var user = await db.Users.FirstOrDefaultAsync(item => item.Id == result.UserId);
            result.UserFullName = user is null ? string.Empty : user.Fullname;
            result.UserAvatar = user is null ? string.Empty : user.Avatar;

            var token = _httpContextAccessor.HttpContext.Request
                .Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization]
                .ToString()
                .Replace("Bearer ", "");

            var usersFriends = await GetFriendIdsByUserIdAsync(user.Id, token);

            var actorInfo = new UserInfo(
                 userId: user.Id,
                 fullName: user.Fullname,
                 avatar: user.Avatar
               );

            var eventBus = _seviceProvider.GetRequiredService<IEventBus>();
            eventBus.Publish(new NotificationIntegrationEvent
                (usersFriends, actorInfo, Guid.Parse(result.Id), NotificationConstants.CreatePost));

            var postImageEntities = await postImageRepository.GetPostImagesAsync(new List<string> { result.Id });

            result.PostImageResponses = postImageEntities?.Select(postImage => new PostImageResponse
            {
                ImageId = postImage.ImageId,
                Url = postImage.Url,
                IsViolence = postImage.IsViolence
            }).ToList() ?? new List<PostImageResponse>();

            var postVideoEntities = await db.PostVideos.Where(x => x.PostId == result.Id).ToListAsync();

            result.PostVideoResponses = postVideoEntities?.Select(postVideo => new PostVideoResponse
            {
                VideoId = postVideo.MediaVideoId,
                Url = postVideo.VideoUrl,
                IsViolence = postVideo.IsViolence
            }).ToList() ?? new List<PostVideoResponse>();
            return result;
        }

        private async Task AddPostToCache(Models.Entities.Post post)
        {
            var cache = _seviceProvider.GetRequiredService<IAppCache>();
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            var userPost = _mapper.Map<GetAllPostResponse>(post);
            var posts = cache.Get<List<GetAllPostResponse>>(CacheKeyConstant.POSTS);

            if (posts is null)
                return;

            var user = await db.Users.FirstOrDefaultAsync(item => item.Id == userPost.UserId);
            userPost.UserFullName = user is null ? string.Empty : user.Fullname;
            userPost.UserAvatar = user is null ? string.Empty : user.Avatar;
            posts.Add(userPost);
        }

        private async Task ValidationCreatePostAsync(CreatePostCommand request)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            var postCategory = await db.PostCategories.AnyAsync(x => x.Id == request.CategoryId);
            if (!postCategory)
            {
                _logger.LogWarning($"Cannot create the post with categoryId={request.CategoryId} not found");
                throw new ApiValidationException("The post category is not found");
            }
        }

        private async Task AddOrUpdateHashtag(string hashtags)
        {
            var postHashtagRepository = _seviceProvider.GetRequiredService<IPostHashtagRepository>();
            if (!string.IsNullOrEmpty(hashtags))
            {
                var lstHashtag = hashtags.Split(",");
                foreach (var item in lstHashtag)
                {
                    var nameHashtag = item.Trim();
                    var lstHashtagByName = await postHashtagRepository.GetByName(nameHashtag);
                    if (lstHashtagByName is null || lstHashtagByName.Count() == 0)
                    {
                        var postHashtag = new PostHashtag()
                        {
                            Id = Guid.NewGuid(),
                            Slug = Utils.ToSlug(nameHashtag),
                            Name = nameHashtag,
                            Description = nameHashtag,
                            IsActived = true,
                            Point = HashtagConstants.POINT_CREATE_POST,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        await postHashtagRepository.CreateAsync(postHashtag);
                    }
                    else
                    {
                        foreach (var hashTag in lstHashtagByName)
                        {
                            TimeSpan ts = DateTime.UtcNow.Subtract(hashTag.UpdatedAt);
                            double numberOfDays = ts.TotalDays;

                            if (numberOfDays <= 7)
                            {
                                hashTag.Point += (HashtagConstants.POINT_HASHTAG_RECENT + HashtagConstants.POINT_CREATE_POST);
                            }
                            else
                            {
                                hashTag.Point += (HashtagConstants.POINT_CREATE_POST);
                            }
                            hashTag.UpdatedAt = DateTime.UtcNow;
                            await postHashtagRepository.UpdateAsync(hashTag);
                        }
                    }
                }
            }
        }

        private async Task<List<Guid>> GetFriendIdsByUserIdAsync(Guid userId, string token)
        {
            try
            {
                var userFriendsRequest = new { UserId = userId, PageNumber = 0, PageSize = 0 };
                var paramJson = JsonConvert.SerializeObject(userFriendsRequest);

                var contentBody = new StringContent(paramJson, Encoding.UTF8, "application/json");

                var url = $"{_configuration["UserService:BaseAddress"]}/v1/user-service/users/get-user-friends";

                using (HttpClient httpClient = new HttpClient())
                {
                    CamelCaseJsonSerializer camelCaseJsonSerializer = new CamelCaseJsonSerializer();

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    HttpResponseMessage response = await httpClient.PostAsync(url, contentBody);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Error in GetFriendIdsByUserIdAsync: {response.ReasonPhrase}");
                        return new List<Guid>();
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = camelCaseJsonSerializer.Deserialize<ApiResponse<UserFriendResponse>>(responseContent);

                    if (apiResponse is null)
                    {
                        _logger.LogError("Response from GetUserFriends Api was null.");
                        return new List<Guid>();
                    }

                    return apiResponse.Data.Select(x => x.UserId).ToList() ?? new List<Guid>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetFriendIdsByUserIdAsync: {ex.Message}");
                throw;
            }
        }
    }
}