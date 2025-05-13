using CabGroupService.Handlers.Interfaces;
using CabGroupService.Infrastructures.Repositories.Interfaces;
using CabGroupService.IntegrationEvents.Events;
using CabGroupService.Models.Dtos;
using CabGroupService.Models.Queries;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace CabGroupService.Handlers.Notification
{
    public partial class NotificationHandler : IQueryHandler<GetNotificationQuery, PagingResponse<NotificationResponse>>
    {
        public async Task<PagingResponse<NotificationResponse>> Handle(GetNotificationQuery request, CancellationToken cancellationToken)
        {
            var response = new PagingResponse<NotificationResponse>();
            var notificationUserIdRepository = _seviceProvider.GetRequiredService<INotificationUserIdMaterializedViewRepository>();

            byte[] pagingStateByte = string.IsNullOrEmpty(request.PagingState) ? null : Convert.FromBase64String(request.PagingState);

            var (data, pagingState) = await notificationUserIdRepository
                .GetListPagingAsync(x => x.UserId == request.UserId, request.PageSize, pagingStateByte);

            if (!data.Any())
                return response;

            var token = _httpContextAccessor.HttpContext.Request
                .Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization]
                .ToString()
                .Replace("Bearer ", "");

            var actorIds = data.Select(x => x.ActorId).Distinct().ToList();
            var actorsInfo = await GetUserInfoByIdsAsync(actorIds, token);

            if (!actorsInfo.Any())
                return response;

            var notificationsResponse = data.Select(notification => new NotificationResponse
            {
                Actor = actorsInfo.FirstOrDefault(x => x.UserId == notification.ActorId),
                ReferenceId = notification.ReferenceId,
                Message = notification.Message,
                NotificationType = notification.NotificationType,
                CreatedAt = notification.CreatedAt,
            }).ToList();


            response.Elements = notificationsResponse;
            response.PageSize = request.PageSize;
            response.PageNumber = request.PageNumber;
            response.Total = await notificationUserIdRepository.CountAsync(x => x.UserId == request.UserId);

            return response;

        }

        private async Task<List<UserInfo>> GetUserInfoByIdsAsync(List<Guid> lstId, string token)
        {
            List<UserInfo> userInfoList = new List<UserInfo>();

            try
            {
                var userIdsJson = JsonConvert.SerializeObject(lstId);

                var contentBody = new StringContent(userIdsJson, Encoding.UTF8, "application/json");

                var url = $"{_configuration["GlobalService:BaseAddress"]}/v1/user-service/Users/ids";

                using (HttpClient _client = new HttpClient())
                {

                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    HttpResponseMessage apiResponse = await _client.PostAsync(url, contentBody);

                    if (!apiResponse.IsSuccessStatusCode)
                    {
                        _logger.LogError($"Error in GetFriendIdsByUserIdAsync: {apiResponse.ReasonPhrase}");
                        return userInfoList;
                    }

                    var responseContent = (await apiResponse.Content.ReadAsStringAsync()).ToString();
                    userInfoList = JsonConvert.DeserializeObject<List<UserInfo>>(responseContent);

                    return userInfoList;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetUserInfoByIdsAsync: {ex.Message}");
                return userInfoList;
            }
        }

    }
}
