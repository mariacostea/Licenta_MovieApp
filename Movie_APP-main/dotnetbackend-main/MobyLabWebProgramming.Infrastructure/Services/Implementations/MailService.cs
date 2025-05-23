using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ServiceResponse> SendMail(
            string recipientEmail,
            string subject,
            string body,
            bool isHtmlBody = false,
            string? senderTitle = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var host = _configuration["Mailtrap:Host"];
                var port = int.Parse(_configuration["Mailtrap:Port"]);
                var user = _configuration["Mailtrap:User"];
                var pass = _configuration["Mailtrap:Pass"];

                using var smtpClient = new SmtpClient(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(user, pass),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("noreply@movieapp.com", senderTitle ?? "MovieApp"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtmlBody
                };

                mailMessage.To.Add(recipientEmail);
                
                await smtpClient.SendMailAsync(mailMessage);

                return ServiceResponse.ForSuccess();
            }
            catch
            {
                return ServiceResponse.FromError(new(
                    HttpStatusCode.ServiceUnavailable,
                    "Mail couldn't be send!",
                    ErrorCodes.MailSendFailed
                ));
            }
        }
    }
}
