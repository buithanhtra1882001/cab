using CabUserService.Models.Dtos;

namespace CabUserService.Services.Interfaces
{
    public interface IDonateReceiverRequestService
    {
        Task<DonateReceiverRequestResponse> CreateRequestAsync(Guid id, DonateReceiverRequestDto requestDto);
        Task<DonateReceiverRequestResponse> GetRequestAsync(Guid id);
        Task<List<DonateReceiverRequestResponse>> GetAllRequestsAsync();
        Task<DonateReceiverRequestResponse> UpdateRequestAsync(Guid userId, DonateReceiverRequestDto requestDto);
        Task DeleteRequestAsync(Guid userId, Guid id);
        Task ApproveRequest(Guid id, Guid userToBeApproveId);
    }
}
