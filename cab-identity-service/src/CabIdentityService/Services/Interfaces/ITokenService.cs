using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.Infrastructures.Token;
using WCABNetwork.Cab.IdentityService.Models.Entities;

namespace WCABNetwork.Cab.IdentityService.Services.Interfaces
{
    public interface ITokenService
    {
        Task<JwtTokenModel> GenerateAsync(Account account);
        Task<JwtTokenModel> RefreshAsync(string token);
        Task<bool> RevokeAsync(string token);
    }
}
