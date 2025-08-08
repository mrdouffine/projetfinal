namespace GestionConge.Components.Services.ServicesImpl;

using GestionConge.Components.DTOs;
using GestionConge.Components.DTOs.RequestDto;
using GestionConge.Components.Models;
using GestionConge.Components.Repositories.IRepositories;
using GestionConge.Components.Services.IServices;

public class ValidationService : IValidationService
{
    private readonly IValidationRepository _validationRepo;
    private readonly IDemandeCongeRepository _demandeRepo;
    private readonly IUtilisateurRepository _utilisateurRepo;
    private readonly IMailService _mailService;

    public ValidationService(
        IValidationRepository validationRepo,
        IDemandeCongeRepository demandeRepo,
        IUtilisateurRepository utilisateurRepo,IMailService mailService)
    {
        _validationRepo = validationRepo;
        _demandeRepo = demandeRepo;
        _utilisateurRepo = utilisateurRepo;
        _mailService = mailService;
    }

    public Task<IEnumerable<ValidationDto>> GetAllAsync() => _validationRepo.GetAllAsync();
    public Task<ValidationDto?> GetByIdAsync(int id) => _validationRepo.GetByIdAsync(id);
    public Task<int> CreateAsync(Validation validation) => _validationRepo.CreateAsync(validation);
    public Task<bool> UpdateAsync(Validation validation) => _validationRepo.UpdateAsync(validation);


    public async Task<bool> TraiterValidationAsync(ValidationRequestDto dto)
    {
        // création validation actuelle
        var validation = new Validation
        {
            DemandeCongeId = dto.DemandeCongeId,
            ValideurId = dto.ValideurId,
            Statut = dto.Statut,
            Commentaire = dto.Commentaire,
            DateValidation = DateTime.UtcNow
        };

        await _validationRepo.CreateAsync(validation);

        var demande = await _demandeRepo.GetByIdAsync(dto.DemandeCongeId);
        if (demande == null) return false;

        var demandeur = await _utilisateurRepo.GetByIdAsync(demande.UtilisateurId);
        if (demandeur == null) return false;

        if (dto.Statut == "Rejeté")
        {
            demande.Statut = "Rejeté";
            await _demandeRepo.UpdateAsync(new DemandeCongeDto
            {
                Id = demande.Id,
                Statut = "Rejeté",
                DateDebut = demande.DateDebut,
                DateFin = demande.DateFin,
                Motif = demande.Motif,
                UtilisateurId = demande.UtilisateurId
            });

            // Envoi email au demandeur
            MailData mailData = new MailData
            {
                To = demandeur.Email,
                Subject = "Demande de congé rejetée",
                Body = $"Bonjour {demandeur.Nom}, votre demande de congé a été rejetée. Commentaire : {dto.Commentaire}"
            };
            await _mailService.SendEmailAsync(mailData);

            return true;
        }

        var valideur = await _utilisateurRepo.GetByIdAsync(dto.ValideurId);
        if (valideur == null) return false;

        if (valideur.Role == "RH" || valideur.Role == "DOT")
        {
            demande.Statut = "Validé";
            await _demandeRepo.UpdateAsync(new DemandeCongeDto
            {
                Id = demande.Id,
                Statut = "Validé",
                DateDebut = demande.DateDebut,
                DateFin = demande.DateFin,
                Motif = demande.Motif,
                UtilisateurId = demande.UtilisateurId
            });

            // Envoi email au demandeur
            MailData mailData = new MailData
            {
                To = demandeur.Email,
                Subject = "Demande de congé validée",
                Body = $"Bonjour {demandeur.Nom}, votre demande de congé a été validée."
            };
            await _mailService.SendEmailAsync(mailData);

            return true;
        }

        // sinon créer la validation pour le supérieur (pas de mail ici)

        var superieur = await _utilisateurRepo.GetByIdAsync(valideur.SuperieurId);
        if (superieur == null) return false;

        await _validationRepo.CreateAsync(new Validation
        {
            DemandeCongeId = dto.DemandeCongeId,
            ValideurId = superieur.Id,
            Statut = "En attente"
        });

        return true;
    }


    public Task<bool> DeleteAsync(int id) => _validationRepo.DeleteAsync(id);
}
