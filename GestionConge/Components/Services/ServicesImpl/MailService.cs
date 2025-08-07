using GestionConge.Components.Services.IServices;
using GestionConge.Components.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GestionConge.Components.Services.ServicesImpl;

public class MailService : IMailService
{
    private readonly MailSettings _mailSettings;

    public MailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

        public async Task SendEmailAsync(MailData mailData)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.SenderEmail);
            email.To.Add(MailboxAddress.Parse(mailData.To));
            email.Subject = mailData.Subject;

            var builder = new BodyBuilder { HtmlBody = mailData.IsHtml ? mailData.Body : null, TextBody = !mailData.IsHtml ? mailData.Body : null };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.SmtpServer, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.SenderEmail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
