using GestionConge.Components.Models;

namespace GestionConge.Components.Services.IServices;

public interface IMailService
{
    Task SendEmailAsync(MailData mailData);
}
