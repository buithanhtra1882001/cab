using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.Infrastructures.Token;
using WCABNetwork.Cab.IdentityService.Models.Dtos;
using WCABNetwork.Cab.IdentityService.Models.Dtos.Requests;

namespace WCABNetwork.Cab.IdentityService.Services.Interfaces
{
    public interface IAccountService
    {
        Task ResendConfirmationEmailAsync(string email);
        Task ConfirmEmailAsync(string userId, string token);
        bool CheckByEmail(string email);
        Task<string> RegisterAsync(RegisterRequest userRegisterRequest);
        Task<LoginReturnType> LoginAsync(LoginRequest userLoginRequest);
        Task RequestPasswordResetAsync(ResetPasswordRequest resetPasswordRequest);
        Task SetNewPasswordAsync(SetNewPasswordRequest setNewPasswordRequest);
        Task ChangePasswordAsync(ChangePasswordRequest changePasswordRequest);
        Task<JwtTokenModel> FacebookLoginAsync(ExternalLoginRequest externalLoginRequest);
        Task<JwtTokenModel> GoogleLoginAsync(ExternalLoginRequest externalLoginRequest);
    }
}
