using CabUserService.Models.Dtos;

namespace CabUserService.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(SendEmailConfig emailConfig);
    }
}
