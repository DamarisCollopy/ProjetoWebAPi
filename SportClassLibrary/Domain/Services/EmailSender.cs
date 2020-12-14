using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using System;

namespace Domain.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly IHostEnvironment host;
        public EmailSender(EmailConfiguration emailConfig, IHostEnvironment host)
        {
            _emailConfig = emailConfig;
            this.host = host;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailConfig.SenderName, _emailConfig.From));
                message.To.Add(new MailboxAddress(email));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = htmlMessage };

                using (var client = new SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    if (host.IsDevelopment())
                    {
                        await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    }
                    else
                    {
                        await client.ConnectAsync(_emailConfig.SmtpServer);
                    }

                    await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }


    }
}
