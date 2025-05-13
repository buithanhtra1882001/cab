using CAB.BuildingBlocks.EventBus.Abstractions;
using CabPostService.Common.Providers;
using CabPostService.Constants;
using CabPostService.Cqrs.Requests.Commands;
using CabPostService.Cqrs.Requests.Queries;
using CabPostService.EventHub.SignalRHub;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Extensions;
using CabPostService.IntegrationEvents.Events;
using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using CabPostService.Services.Abstractions;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using IUserService = CabPostService.Grpc.Procedures.IUserService;

namespace CabUserService.Services
{
    public class DonateService : IDonateService
    {
        #region Properties
        protected readonly IServiceProvider _seviceProvider;
        protected readonly ILogger<DonateService> _logger;
        protected readonly ScyllaDbContext _scyllaDbContext;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<DonateHub> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly HubConnection _connection;

        #endregion

        #region Constructor

        public DonateService(ScyllaDbContext scyllaDbContext, IHttpContextAccessor httpContextAccessor, IHubContext<DonateHub> hubContext,
            IConfiguration configuration, IUserService userService, IServiceProvider seviceProvider)
        {
            _scyllaDbContext = scyllaDbContext;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
            _configuration = configuration;
            _userService = userService;
            _seviceProvider = seviceProvider;
            _connection = new HubConnectionBuilder()
            .WithUrl("https://api.devcab.org/hubs/donate", options =>
            {
                options.SkipNegotiation = true;
                options.Transports = HttpTransportType.WebSockets;
            })
            .Build();
        }

        #endregion

        #region Methods

        public async Task<ResponseDto> DonateReceiverAsync(DonateReceiverCommand command, CancellationToken cancellationToken = default)
        {
            ResponseDto responseDto = new ResponseDto();
            var token = _httpContextAccessor.HttpContext.Request
                .Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization]
                .ToString()
                .Replace("Bearer ", "");

            var donaterId = await GetUuidByTokenAsync(token);

            if (donaterId == command.ReceiverId)
                throw new ApiValidationException("You can't donate to yourself");

            var users = await GetUserInfoByIdsAsync(new List<Guid>() { donaterId, command.ReceiverId }, token);
            if (!users.Any())
                throw new ApiValidationException("Donater and Receiver does not exist");

            if (users.Any() && users.FirstOrDefault(x => x.UserId == donaterId) == null)
                throw new ApiValidationException("Donater does not exist");

            if (users.Any() && users.FirstOrDefault(x => x.UserId == command.ReceiverId) == null)
                throw new ApiValidationException("Receiver does not exist");

            var donater = users.FirstOrDefault(x => x.UserId == donaterId);
            if (donater != null && donater.Coin < command.Coin)
                throw new ApiValidationException($"{donater.Fullname} Insufficient balance in the account");

            // send message to RabbitMQ when have user donate for creator                

            command.Token = token;
            SendMessageToRabbitMQ(command, cancellationToken);
            responseDto.Status = HttpStatusCode.OK;

            return responseDto;
        }

        public async Task<bool> HandleDonateReceiverAsync(HandleDonateReceiverCommand command, CancellationToken cancellationToken = default)
        {
            ResponseDto responseDto = new ResponseDto();
            var eventBus = _seviceProvider.GetRequiredService<IEventBus>();
            var status = false;
            try
            {
                command.DonaterId = await GetUuidByTokenAsync(command.Token);
                // Save to DB
                var isDonate = await SaveToDatabase(command);

                var users = await GetUserInfoByIdsAsync(new List<Guid>() { command.DonaterId, command.ReceiverId }, command.Token);
                var donater = users.FirstOrDefault(x => x.UserId == command.DonaterId);
                var receiver = users.FirstOrDefault(x => x.UserId == command.ReceiverId);

                if (!isDonate)
                {
                    responseDto.Status = HttpStatusCode.BadGateway;
                    // Used signalR sent notice to client
                    await NotifyClientsAsync(new DonateReceiverResponse()
                    {
                        Donater = donater.Fullname,
                        Receiver = receiver.Fullname,
                        Content = command.Content,
                        Title = command.Title,
                        Coin = command.Coin,
                        Status = HttpStatusCode.BadRequest
                    });
                }
                else
                {
                    var actorInfo = new UserInfo(
                         userId: donater.UserId,
                         fullName: donater.Fullname,
                         avatar: donater.Avatar
                        );

                    eventBus.Publish(new NotificationIntegrationEvent
                        (new List<Guid> { receiver.UserId }, actorInfo, Guid.Parse(command.PostId), NotificationConstants.DonatePost, command.Coin));

                    // Used signalR sent notice to client
                    await NotifyClientsAsync(new DonateReceiverResponse()
                    {
                        Donater = donater.Fullname,
                        Receiver = receiver.Fullname,
                        Content = command.Content,
                        Title = command.Title,
                        Coin = command.Coin,
                        Status = HttpStatusCode.OK
                    });

                    status = true;
                }
            }
            catch (Exception ex)
            {
                responseDto.Status = HttpStatusCode.BadGateway;
                responseDto.Message = ex.Message;
            }
            return status;
        }

        public async Task<StatisticalDonateDto> HandlesSatisticalDonateAsync(StatisticalDonateQuery request, CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext.Request
        .Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization]
        .ToString()
        .Replace("Bearer ", "");
            var donaterId = await GetUuidByTokenAsync(token);
            if (request.UserId.HasValue && request.UserId.Value != Guid.Empty)
            {
                donaterId = request.UserId.Value;
            }
            var querytopMoney = await _scyllaDbContext.GetTable<PostDonate>().ExecuteAsync();
            var querytopAction = await _scyllaDbContext.GetTable<PostDonate>().ExecuteAsync();

            querytopMoney = querytopMoney.Where(x => x.ReceiverId == donaterId);
            querytopAction = querytopAction.Where(x => x.ReceiverId == donaterId);

            if (request.FromDate != null)
            {
                querytopMoney = querytopMoney.Where(x => request.FromDate >= x.CreatedAt);
                querytopAction = querytopAction.Where(x => request.FromDate >= x.CreatedAt);
            }

            if (request.ToDate != null)
            {
                querytopMoney = querytopMoney.Where(x => request.ToDate <= x.CreatedAt);
                querytopAction = querytopAction.Where(x => request.ToDate <= x.CreatedAt);
            }

            var lstTopMoney = querytopMoney
                .GroupBy(x => x.DonaterId)
                .Select(group => new TopMoney
                {
                    UserId = group.Key,
                    TotalAmount = group.Sum(x => x.Value)
                })
                .OrderByDescending(g => g.TotalAmount)
                .ToList();

            var lstTopAction = querytopAction
                .GroupBy(x => x.DonaterId)
                .Select(group => new TopAction
                {
                    UserId = group.Key,
                    TotalAction = group.Count()
                })
                .OrderByDescending(g => g.TotalAction)
                .ToList();

            var lstUuid = new List<Guid>();
            if (lstTopMoney != null)
            {
                var lstUserId = lstTopMoney.Select(x => x.UserId).ToList();
                lstUuid = lstUuid.Concat(lstUserId).ToList();
            }

            if (lstTopAction != null)
            {
                var lstUserId = lstTopAction.Select(x => x.UserId).ToList();
                lstUuid = lstUuid.Concat(lstUserId).ToList();
            }

            if (lstUuid.Any())
            {
                var users = await GetUserInfoByIdsAsync(lstUuid, token);
                if (users.Any())
                {
                    return new StatisticalDonateDto()
                    {
                        TopMoney = lstTopMoney.Select(x => new TopMoney()
                        {
                            UserId = x.UserId,
                            TotalAmount = x.TotalAmount,
                            Avatar = users.FirstOrDefault(k => k.UserId == x.UserId)?.Avatar,
                            UserName = users.FirstOrDefault(k => k.UserId == x.UserId)?.Username
                        }).ToList(),
                        TopAction = lstTopAction.Select(x => new TopAction()
                        {
                            UserId = x.UserId,
                            TotalAction = x.TotalAction,
                            Avatar = users.FirstOrDefault(k => k.UserId == x.UserId)?.Avatar,
                            UserName = users.FirstOrDefault(k => k.UserId == x.UserId)?.Username
                        }).ToList()
                    };
                }
            }

            return new StatisticalDonateDto()
            {
                TopMoney = lstTopMoney,
                TopAction = lstTopAction
            };

        }

        public async Task<List<GetLstReceiveAmountsByIdResponse>> GetLstReceiveAmountsByIdAsync(GetLstReceiveAmountsByIdQuery request, CancellationToken cancellationToken)
        {
            //var token = _httpContextAccessor.HttpContext.Request
            //        .Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization]
            //        .ToString()
            //        .Replace("Bearer ", "");

            //request.UserId = await GetUuidByTokenAsync(token);
            var queryDonater = await _scyllaDbContext.GetTable<PostDonate>().ExecuteAsync();
            var queryReceiver = await _scyllaDbContext.GetTable<PostDonate>().ExecuteAsync();

            var lstDonaterById = queryDonater
            //.Where(x => x.ReceiverId == request.UserId)
            .GroupBy(u => u.DonaterId)
            .Select(g => new GetLstReceiveAmountsByIdResponse { DonaterId = g.Key, TotalAmount = g.Sum(x => x.Value) })
            .OrderByDescending(x => x.TotalAmount)
            .Take(10)
            .ToList();

            var lstReceiverById = queryReceiver
            //.Where(x => x.DonaterId == request.UserId)
            .GroupBy(u => u.ReceiverId)
            .Select(g => new GetLstReceiveAmountsByIdResponse { ReceiverId = g.Key, TotalAmount = g.Sum(x => x.Value) })
            .OrderByDescending(x => x.TotalAmount)
            .Take(10)
            .ToList();

            var lstAmountsById = lstDonaterById.Concat(lstReceiverById).ToList();

            return lstAmountsById;
        }

        public async Task<List<DonateDetailResponse>> GetDetailDonateAsync(GetDetailDonateQuery request, CancellationToken cancellationToken)
        {
            var result = new List<DonateDetailResponse>();

            var token = _httpContextAccessor.HttpContext.Request
                    .Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization]
                    .ToString()
                    .Replace("Bearer ", "");
            var userId = await GetUuidByTokenAsync(token);

            try
            {
                var query = await _scyllaDbContext.GetTable<PostDonate>().ExecuteAsync();
                query = query.ToList();

                if (!query.Any())
                    return new List<DonateDetailResponse>();

                var isExistLst = query.Where(x => x.ReceiverId == userId).ToList();

                if (!isExistLst.Any())
                    return new List<DonateDetailResponse>();

                var lstUserId = isExistLst.Select(x => x.DonaterId).Distinct().ToList();
                lstUserId = lstUserId.Concat(isExistLst.Select(x => x.ReceiverId).Distinct().ToList()).ToList();
                var users = await GetUserInfoByIdsAsync(lstUserId, token);
                if (users.Any())
                {
                    result = isExistLst.Select(x => new DonateDetailResponse()
                    {
                        Id = x.Id,
                        Coin = x.Value,
                        Content = x.Content,
                        Title = x.Title,
                        Donater = users.FirstOrDefault(y => y.UserId == x.DonaterId)?.Username,
                        Receiver = users.FirstOrDefault(y => y.UserId == x.ReceiverId)?.Username,
                        CreateDonate = x.CreatedAt

                    }).ToList();
                }
                else
                {
                    result = isExistLst.Select(x => new DonateDetailResponse()
                    {
                        Id = x.Id,
                        Coin = x.Value,
                        Content = x.Content,
                        Title = x.Title,
                        Donater = string.Empty,
                        Receiver = string.Empty,
                        CreateDonate = x.CreatedAt

                    }).ToList();
                }

            }
            catch (Exception)
            {
                throw new Exception($"{HttpStatusCode.InternalServerError}");
            }

            return result;
        }

        private async void SendMessageToRabbitMQ(DonateReceiverCommand message, CancellationToken cancellationToken = default)
        {
            await HandleDonateReceiverAsync(new HandleDonateReceiverCommand()
            {
                Coin = message.Coin,
                Content = message.Content,
                PostId = message.PostId,
                ReceiverId = message.ReceiverId,
                Title = message.Title,
                Token = message.Token
            }, cancellationToken);
            return;
            var _factory = new ConnectionFactory() { HostName = _configuration["RabbitConfig:HostName"] };
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _configuration["RabbitConfig:queue"], durable: false, exclusive: false, autoDelete: false, arguments: null);

                var body = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message));

                channel.BasicPublish(exchange: _configuration["RabbitConfig:exchange"], routingKey: _configuration["RabbitConfig:routingKey"], basicProperties: null, body: body);
            }
        }

        private async Task<bool> SaveToDatabase(HandleDonateReceiverCommand message)
        {
            var isSuccess = false;
            try
            {
                var response = await HandleDonateCreatorAsync(message);

                if (response.Status != HttpStatusCode.OK)
                {
                    isSuccess = false;
                    return isSuccess;
                }

                var postDonate = new PostDonate()
                {
                    Id = Guid.NewGuid(),
                    Content = message.Content,
                    DonaterId = message.DonaterId,
                    PostId = message.PostId,
                    ReceiverId = message.ReceiverId,
                    Title = message.Title,
                    Value = message.Coin,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var donate = await _scyllaDbContext.GetTable<PostDonate>().Insert(postDonate).ExecuteAsync();
                if (donate != null)
                    isSuccess = true;
                //using (var scope = _seviceProvider.CreateScope())
                //{
                //    var postDonateRepository = scope.ServiceProvider.GetRequiredService<IPostDonateRepository>();
                //    var donate = await postDonateRepository.CreateAsync(postDonate);
                //}
            }
            catch (Exception)
            {
                return isSuccess;
            }
            return isSuccess;
        }

        private async Task<ResponseDto> HandleDonateCreatorAsync(HandleDonateReceiverCommand command)
        {
            ResponseDto responseDto = new ResponseDto();

            try
            {
                var paramJson = JsonConvert.SerializeObject(command);

                //create content
                var contentBody = new StringContent(paramJson, Encoding.UTF8, "application/json");

                var url = $"{_configuration["UserService:BaseAddress"]}/v1/user-service/Users/handle-donate";
                //var url = $"http://localhost:9002/api/v1/Users/handle-donate";

                using (HttpClient _client = new HttpClient())
                {
                    CamelCaseJsonSerializer camelCaseJsonSerializer = new CamelCaseJsonSerializer();
                    var token = command.Token;

                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage response = await _client.PostAsync(url, contentBody);
                    responseDto.Status = response.StatusCode;
                    responseDto.Message = (await response.Content.ReadAsStringAsync()).ToString();
                    var resultMsg = camelCaseJsonSerializer.Deserialize<ResponseDto>(responseDto.Message);
                    if (!response.IsSuccessStatusCode)
                    {
                        responseDto.Status = HttpStatusCode.BadRequest;
                        return responseDto;
                    }
                    if (resultMsg.Status != HttpStatusCode.OK)
                    {
                        responseDto.Status = HttpStatusCode.BadRequest;
                        return responseDto;
                    }

                    var results = JsonConvert.DeserializeObject<StatisticalUserDto>(responseDto.Message);
                    return responseDto;
                }
            }
            catch (Exception ex)
            {
                responseDto.Status = HttpStatusCode.BadGateway;
                responseDto.Message = ex.Message;
                return responseDto;
            }
        }

        private async Task<List<PublicUserInformationResponse>> GetUserInfoByIdsAsync(List<Guid> lstId, string token)
        {
            List<PublicUserInformationResponse> responseDto = new List<PublicUserInformationResponse>();

            try
            {
                var paramJson = JsonConvert.SerializeObject(lstId);

                //create content
                var contentBody = new StringContent(paramJson, Encoding.UTF8, "application/json");

                var url = $"{_configuration["UserService:BaseAddress"]}/v1/user-service/Users/ids";
                //var url = $"http://localhost:9002/api/v1/Users/ids";

                using (HttpClient _client = new HttpClient())
                {
                    CamelCaseJsonSerializer camelCaseJsonSerializer = new CamelCaseJsonSerializer();

                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    HttpResponseMessage response = await _client.PostAsync(url, contentBody);

                    if (!response.IsSuccessStatusCode)
                        return responseDto;

                    var messageResult = (await response.Content.ReadAsStringAsync()).ToString();
                    responseDto = camelCaseJsonSerializer.Deserialize<List<PublicUserInformationResponse>>(messageResult);
                    return responseDto;
                }
            }
            catch (Exception ex)
            {
                return responseDto;
            }
        }

        private async Task NotifyClientsAsync(DonateReceiverResponse message)
        {
            try
            {
                await _connection.StartAsync();
                await _connection.InvokeAsync("SendDonateNotification", message);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private async Task<Guid> GetUuidByTokenAsync(string token)
        {
            var uuid = Guid.NewGuid();

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var uuidClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Uuid");

            if (uuidClaim != null)
                uuid = Guid.Parse(uuidClaim.Value);

            return uuid;
        }
    }

    #endregion
}
