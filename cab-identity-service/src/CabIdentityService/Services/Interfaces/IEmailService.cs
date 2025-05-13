using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.Models.Dtos.Mail;

namespace WCABNetwork.Cab.IdentityService.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(SendEmailConfig sendEmailConfig);
    }
}
