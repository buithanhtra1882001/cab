using CabUserService.Models.Dtos;

namespace CabUserService.Services.Interfaces
{
    public interface IWithdrawalRequestService
    {
        Task<WithdrawalRequestResponse> CreateRequestAsync(Guid id, WithdrawalRequestDto requestDto);
        Task<WithdrawalRequestResponse> GetPendingRequestAsync(Guid id);
        Task<List<WithdrawalRequestResponse>> GetAllRequestsAsync();
        Task<Guid?> ApproveRequest(Guid requestId);
    }
}
