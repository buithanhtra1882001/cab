using AutoMapper;
using CabPostService.Cqrs.Requests.Queries;
using CabPostService.Grpc.Procedures;
using CabPostService.Infrastructures.Communications.Http;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Models.Dtos;
using CabPostService.Services.Base;
using CabPostService.Services.Abstractions;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using CabPostService.Constants;
using CabPostService.Infrastructures.Repositories.Interfaces;
using Newtonsoft.Json;
using Serilog;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore.Update;
using CabPostService.Common.Providers;
using CabPostService.Models.Entities;
using MediatR;
using System.Net.Http;
using System.Net.Http.Headers;
using CabPostService.Cqrs.Requests.Commands;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;

namespace CabUserService.Services
{
    public class UserService : CabPostService.Services.Abstractions.IUserService
    {
        private IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly IAppCache _cache;
        private readonly PostgresDbContext _postgresDbContext;
        protected readonly ILogger<UserService> _logger;
        protected readonly ScyllaDbContext _scyllaDbContext;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(
            ILogger<UserService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IAppCache cache,
            IMapper mapper,
            IHttpClientWrapper httpClientWrapper,
            PostgresDbContext postgresDbContext,
            ScyllaDbContext scyllaDbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _cache = cache;
            _mapper = mapper;
            _httpClientWrapper = httpClientWrapper;
            _postgresDbContext = postgresDbContext;
            _scyllaDbContext = scyllaDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<StatisticalUserDto> GetStatisticalUserAsync(StatisticalUserQuery request, CancellationToken cancellationToken)
        {
            StatisticalUserDto result = new StatisticalUserDto();

            // get like
            var queryLike = _postgresDbContext.PostVotes.AsNoTracking()
                .Where(x => x.Post.UserId == request.UserId).AsQueryable();

            if (request.FromDate != null)
            {
                queryLike = queryLike.Where(x => request.FromDate >= x.CreatedAt).AsQueryable();
            }

            if (request.ToDate != null)
            {
                queryLike = queryLike.Where(x => request.ToDate <= x.CreatedAt).AsQueryable();
            }

            var countLike = await queryLike.Where(x => x.Type == PostConstant.VOTE_UP).ToListAsync(cancellationToken);

            // get dislike            
            var countDislike = await queryLike.Where(x => x.Type == PostConstant.VOTE_DOWN).ToListAsync(cancellationToken);

            // get comments
            var queryComment = await _scyllaDbContext.GetTable<PostComment>().ExecuteAsync();
            queryComment = queryComment.Where(item => item.UserId == request.UserId);
            if (request.FromDate != null)
            {
                queryComment = queryComment.Where(x => request.FromDate >= x.CreatedAt);
            }

            if (request.ToDate != null)
            {
                queryComment = queryComment.Where(x => request.ToDate <= x.CreatedAt);
            }

            // get posts
            var queryPost = _postgresDbContext.Posts.AsNoTracking()
                .Where(x => x.UserId == request.UserId).AsQueryable();

            if (request.FromDate != null)
            {
                queryPost = queryPost.Where(x => request.FromDate >= x.CreatedAt).AsQueryable();
            }

            if (request.ToDate != null)
            {
                queryPost = queryPost.Where(x => request.ToDate <= x.CreatedAt).AsQueryable();
            }

            var countPost = await queryPost.ToListAsync(cancellationToken);

            // get top reac
            var mostActiveUser = queryLike.GroupBy(x => x.UserVoteId)
            .Select(group => new TopReaction
            {
                name = group.Key.ToString(),
                count = group.Count()
            })
            .OrderByDescending(g => g.count)
            .FirstOrDefault();

            // get top donate
            var queryDonate = await _scyllaDbContext.GetTable<PostDonate>().ExecuteAsync();
            queryDonate = queryDonate.Where(x => x.DonaterId == request.UserId);

            if (request.FromDate != null)
            {
                queryDonate = queryDonate.Where(x => request.FromDate >= x.CreatedAt);
            }

            if (request.ToDate != null)
            {
                queryDonate = queryDonate.Where(x => request.ToDate <= x.CreatedAt);
            }

            var topDonate = queryDonate
            .GroupBy(x => x.DonaterId)
            .Select(group => new TopDonate
            {
                name = group.Key.ToString(),
                totalValue = group.Sum(x => x.Value)
            })
            .OrderByDescending(g => g.totalValue)
            .FirstOrDefault();

            // get new friend
            var friend = new StatisticalUserDto();
            string topDonaterId = string.Empty;
            string topMostActiveUserId = string.Empty;

            if (topDonate != null && !string.IsNullOrEmpty(topDonate.name))
                topDonaterId = topDonate.name;
            if (mostActiveUser != null && !string.IsNullOrEmpty(mostActiveUser.name))
                topMostActiveUserId = mostActiveUser.name;

            friend = await GetFriendByIdAsync(request, topDonaterId, topMostActiveUserId);

            if(topDonate != null)
                topDonate.name = friend?.topDonate?.name;
            if(mostActiveUser != null)
                mostActiveUser.name = friend?.topReaction?.name;

            return new StatisticalUserDto()
            {
                like = countLike.Count(),
                disLike = countDislike.Count(),
                comment = queryComment.Count(),
                newFriend = friend.newFriend,
                post = countPost.Count(),
                topDonate = topDonate,
                topReaction = mostActiveUser
            };
        }

        private async Task<StatisticalUserDto> GetFriendByIdAsync(StatisticalUserQuery query, string topDonater, string topReaction)
        {
            ResponseDto responseDto = new ResponseDto();
            StatisticalUserDto result = new StatisticalUserDto();

            try
            {
                var paramJson = JsonConvert.SerializeObject(query);

                //create content
                var contentBody = new StringContent(paramJson, Encoding.UTF8, "application/json");

                var endpoint = $"{_configuration["UserService:BaseAddress"]}/v1/user-service/Users/statistical-user";
                var url = $"{endpoint}?UserId={query.UserId}&DonaterId={topDonater}&ReactionId={topReaction}&FromDate={query.FromDate}&ToDate={query.ToDate}";

                using (HttpClient _client = new HttpClient())
                {
                    var token = _httpContextAccessor.HttpContext.Request
                    .Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization]
                    .ToString()
                    .Replace("Bearer ", "");

                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await _client.GetAsync(url);
                    responseDto.Status = response.StatusCode;
                    responseDto.Message = (await response.Content.ReadAsStringAsync()).ToString();
                    if (!response.IsSuccessStatusCode)
                    {
                        return result;
                    }

                    result = JsonConvert.DeserializeObject<StatisticalUserDto>(responseDto.Message);

                    return result;
                }
            }
            catch (Exception ex)
            {
                responseDto.Status = HttpStatusCode.BadGateway;
                responseDto.Message = ex.Message;
                return result;
            }
        }
    }
}
