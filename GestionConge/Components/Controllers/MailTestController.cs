namespace GestionConge.Components.Controllers;

using GestionConge.Components.Models;
using GestionConge.Components.Services.IServices;
using Microsoft.AspNetCore.Mvc;
    using Org.BouncyCastle.Asn1.Pkcs;
    using System.Threading.Tasks;

   
        [Route("api/[controller]")]
        [ApiController]
        public class MailTestController : ControllerBase
        {
            private readonly IMailService _mailService;

            public MailTestController(IMailService mailService)
            {
                _mailService = mailService;
            }

            [HttpPost("send-test")]
            public async Task<IActionResult> SendTestEmail()
            {
                var email = new MailData
                {
                    To = "kojjey2@gmail.com", // change ça avec ton adresse de test
                    Subject = "Test d'envoi d'email",
                    Body = "<h1>Bonjour !</h1><p>Ceci est un email de test envoyé depuis GestionCongé 🚀</p>",
                    IsHtml = true
                };

                await _mailService.SendEmailAsync(email);
                return Ok("Email envoyé !");
            }
        }
    


