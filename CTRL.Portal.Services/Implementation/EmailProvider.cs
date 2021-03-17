using CTRL.Portal.Common.Contracts;
using CTRL.Portal.Services.Configuration;
using CTRL.Portal.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace CTRL.Portal.Services.Implementation
{
    public class EmailProvider : IEmailProvider
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IViewRenderService _viewRenderService;

        public EmailProvider(EmailConfiguration emailConfiguration, IViewRenderService viewRenderService)
        {
            _emailConfiguration = emailConfiguration ?? throw new ArgumentNullException(nameof(emailConfiguration));
            _viewRenderService = viewRenderService ?? throw new ArgumentNullException(nameof(viewRenderService));
        }

        public async Task SendEmail<TEmailContract>(TEmailContract email) 
            where TEmailContract : EmailContract
        {
            if(!(email is EmailContract contract))
            {
                throw new ArgumentException($"Email model must be of type {nameof(EmailContract)}", nameof(email));
            }

            ValidateEmail(contract);

            try
            {
                var message = new MimeMessage();

                var sender = new MailboxAddress(_emailConfiguration.SenderName, _emailConfiguration.SenderUrl);
                message.From.Add(sender);

                var recipient = new MailboxAddress(email.Name, email.Recipient);
                message.To.Add(recipient);

                message.Subject = email.Header;

                var body = await _viewRenderService.RenderToStringAsync(email.ViewName, email);

                var builder = new BodyBuilder();
                builder.HtmlBody = body;
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
            if (email is null) throw new ArgumentException(nameof(email));
            if (string.IsNullOrWhiteSpace(email.Header)) throw new ArgumentException(nameof(email.Header));
            if (string.IsNullOrWhiteSpace(email.Name)) throw new ArgumentException(nameof(email.Name));
            if (string.IsNullOrWhiteSpace(email.Recipient)) throw new ArgumentException(nameof(email.Recipient));
        }
    }
}
