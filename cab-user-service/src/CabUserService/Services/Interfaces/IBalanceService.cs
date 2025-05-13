using CabUserService.Cqrs.Requests.Commands;
using CabUserService.Cqrs.Requests.Dto.Donates;
using CabUserService.Infrastructures.DbContexts;
using CabUserService.Models.Dtos;
using System.Drawing;

namespace CabUserService.Services.Interfaces
{
    public interface IBalanceService
    {
        Task<Guid?> UserDonataionAsync(Guid auid, UserDonationRequest request, Guid? postId);
        Task<string> GetDonateURLParam(Guid auid);
        Task<PagingResponse<UserBalanceLogDto>> GetUserBalanceLogAsync(Guid auid, GetUserBalanceLogRequest request);
        Task<PagingResponse<UserTransactionDto>> GetUserTransactionLogAsync(Guid auid, GetUserTransactionRequest request);
        Task<UserTransactionDto> GetUserTransactionByIdAsync(Guid auid, GetUserTransactionByIdRequest request);
        Task<double> GetTotalDonateAsync(Guid auid, TotalDonateRequest request);
        Task<List<DonatorResponse>> GetTopDonatorAsync(Guid auid, TotalDonateRequest request);
        Task<bool> CreatorHideOrShowDonationContentAsync(Guid auid, Guid transactionId);
        Task<Guid> UseBalanceAsync(UseBalanceRequest request, PostgresDbContext db = null);
        Task<Guid> AddUserTransactionAsync(
            AddUserTransactionRequest request,
            PostgresDbContext db = null);
    }
}
