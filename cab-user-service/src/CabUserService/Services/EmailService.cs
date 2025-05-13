using CabUserService.Models.Dtos;
using CabUserService.Services.Base;
using CabUserService.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MediatR;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CabUserService.Services
{
    public class EmailService : BaseService<EmailService>, IEmailService
    {
        private readonly IMediator _mediator;
        private readonly AppSettings _appSettings;

        public EmailService(ILogger<EmailService> logger,
             IOptions<AppSettings> options,
             IMediator mediator)
             : base(logger)
        {
            _appSettings = options.Value;
            _mediator = mediator;
        }

        public async Task SendAsync(SendEmailConfig sendEmailConfig)
        {
            var message = new MimeMessage();
            var builder = new BodyBuilder();

            builder.HtmlBody = sendEmailConfig.Body;

            message.From.Add(new MailboxAddress(sendEmailConfig.FromName, sendEmailConfig.FromEmail));
            message.To.Add(new MailboxAddress(sendEmailConfig.ToName, sendEmailConfig.ToEmail));
            message.Subject = sendEmailConfig.Subject;
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(
                    _appSettings.STMPSetting.Host,
                    _appSettings.STMPSetting.Port,
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(_appSettings.STMPSetting.Username, _appSettings.STMPSetting.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
