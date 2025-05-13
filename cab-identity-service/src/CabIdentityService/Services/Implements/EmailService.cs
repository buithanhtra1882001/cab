using MailKit.Net.Smtp;
using MailKit.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Serilog;
using System;
using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.Models.Dtos.Mail;
using WCABNetwork.Cab.IdentityService.Services.Base;
using WCABNetwork.Cab.IdentityService.Services.Interfaces;

namespace WCABNetwork.Cab.IdentityService.Services.Implements
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
            try
            {
                using var client = new SmtpClient();

                client.Connect(
                    _appSettings.STMPSetting.Host,
                    _appSettings.STMPSetting.Port,
                   SecureSocketOptions.StartTls);
                client.Authenticate(_appSettings.STMPSetting.Username, _appSettings.STMPSetting.Password);
                await client.SendAsync(message);
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
    }
}