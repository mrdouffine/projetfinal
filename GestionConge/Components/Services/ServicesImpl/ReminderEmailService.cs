namespace GestionConge.Components.Services.ServicesImpl;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using GestionConge.Components.Services.IServices;
using GestionConge.Components.Models;

public class ReminderEmailService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public ReminderEmailService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await SendReminderEmails();

            // Attendre 24h avant la prochaine vérification
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task SendReminderEmails()
    {
        using var scope = _serviceProvider.CreateScope();

        var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        using var db = new NpgsqlConnection(connectionString);

        var dateCible = DateTime.Today.AddDays(3);

        var conges = await db.QueryAsync<dynamic>(
            @"SELECT dc.*, u.nom, u.email 
              FROM demandes_conge dc
              JOIN utilisateurs u ON dc.utilisateurid = u.id
              WHERE dc.date_fin = @DateCible AND dc.statut = 'Validé'",
            new { DateCible = dateCible });

        foreach (var conge in conges)
        {
            string email = conge.email;
            string nom = conge.nom;
            DateTime dateFin = conge.datefin;
            MailData mailData = new MailData
            {
                To = email,
                Subject = "📅 Rappel : Retour de congé dans 3 jours",
                Body = $"Bonjour {nom},<br>Votre congé se termine le <strong>{dateFin:dd/MM/yyyy}</strong>. Préparez votre retour !"
            };

            await mailService.SendEmailAsync(mailData);
        }
    }
}
