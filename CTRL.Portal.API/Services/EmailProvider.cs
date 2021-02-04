using CTRL.Portal.API.Configuration;
using CTRL.Portal.API.Contracts;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;

namespace CTRL.Portal.API.Services
{
    public class EmailProvider : IEmailProvider
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailProvider(EmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration ?? throw new ArgumentNullException(nameof(emailConfiguration));
        }

        public void SendEmail(EmailContract email)
        {
            ValidateEmail(email);

            try
            {
                var message = new MimeMessage();

                var sender = new MailboxAddress(_emailConfiguration.SenderName, _emailConfiguration.SenderUrl);
                message.From.Add(sender);

                var recipient = new MailboxAddress(email.Name, email.Recipient);
                message.To.Add(recipient);

                message.Subject = email.Header;

                var builder = new BodyBuilder();
                builder.TextBody = email.Message;
                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, SecureSocketOptions.StartTls);
                    client.Authenticate(_emailConfiguration.Login, _emailConfiguration.Password);

                    client.Send(message);
                }
            }
            catch(Exception e)
            {
                throw new InvalidOperationException($"Emailer encountered a problem: {e.Message}");
            }
        }

        private void ValidateEmail(EmailContract email)
        {
            
        }
    }
}
