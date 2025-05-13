using CabUserService.Constants;
using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;

namespace CabUserService.Services.Interfaces
{
    public interface IProfileService
    {
        Task<PagingResponse<User>> AdminGetListAsync(GetAllUserRequest request);
        Task<IEnumerable<UserFindByUserNameResponse>> UserGetListAsync(Guid auid, string username);
        Task<PublicUserInformationResponse> UserGetProfileAsync(Guid auid, Guid? cabUserId);
        Task<List<PublicUserInformationResponse>> GetListUserProfileAsync(IEnumerable<Guid> auids);
        Task<PublicUserInformationResponse> AdminGetProfileAsync(Guid auid, string userRole);
        Task UserCreateOrUpdateProfileAsync(Guid auid, UserCreateUpdateRequest userRequest);
        Task<string> AdminDeleteUserAsync(Guid auid, Guid cabUserId);
        Task<string> UpdateAvatarAsync(Guid auid, string avatar, string url);
        Task<string> AddFollowerAsync(Guid userId, string follow);
        Task<string> AddFollowingAsync(Guid userId, string follow);
        Task<string> UnfollowerAsync(Guid userId, Guid followerId);
        Task<TotalFollowResponse> GetTotalFollowAsync(Guid userId);
        Task<Response> GetFollowListAsync(Guid userId, FOLLOW_TYPE type);
        Task<bool> ViewUserProfileAsync(Guid viewerId, Guid profileUserId);

        Task<long> StatisticsProfileViewAsync(Guid auid, IntervalType intervalType);

        Task<InteractionStatisticsResponse> InteractionStatisticsAsync(Guid auid, IntervalType intervalType);
        Task<string> UpdateBackgroundCoverAsync(Guid auid, string avatar, string url);
    }
}