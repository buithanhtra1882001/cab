using CabUserService.Cqrs.Requests.Commands;
using CabUserService.Cqrs.Requests.Dto.Donates;
using CabUserService.Cqrs.Requests.Queries;
using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;

namespace CabUserService.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<CreatorResponse>> GetRequestCreatorAsync(string bearerToken, Guid userId);
        Task<string> RequestCreatorAsync(Guid userId);
        Task<string> ConfirmCreatorAsync(Guid userId);
        Task NotifyUserCreatorsAsync();
        Task<List<UserRequestFriendDto>> GetRequestFriendAsync(Guid userId);
        Task<string> AddFriendRequestAsync(Guid auid, RequestFriendRequest request);
        Task<List<RequestFriendResponse>> GetFriendRequestAsync(Guid auid);
        Task<string> AddFriendAsync(Guid auid, AcceptFriendRequest request);
        Task<string> UnfriendAsync(Guid auid, Guid friendId);
        Task<PagingResponse<UserFriendResponse>> GetUserFriendsAsync(GetUserFriendsRequest request);
        Task<PagingResponse<UserMessageResponse>> GetMessagesByUserIdAsync(GetUserMessagesByUserIdRequest request);
        Task<PagingResponse<UserOnlineResponse>> GetUserFriendOnlineAsync(UserFriendOnlineRequest request);
        Task<bool> IsUserOnlineAsync(Guid id);
        Task<PagingStateResponse<MessagesResponse>> GetContentMessagesAsync(GetContentMessageRequest request);
        Task<LeaderBoardResponse> GetLeaderBoardAsync();

        Task<bool> UpdateProfileAsync(UpdateProfileCommand request, CancellationToken cancellationToken = default);

        Task<StatisticalUserDto> GetStatisticalUserAsync(StatisticalUserQuery request, CancellationToken cancellationToken);
    }
}
