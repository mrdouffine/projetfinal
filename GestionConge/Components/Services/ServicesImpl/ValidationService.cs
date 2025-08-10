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
        IUtilisateurRepository utilisateurRepo,
        IMailService mailService)
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
    public Task<bool> DeleteAsync(int id) => _validationRepo.DeleteAsync(id);

    public async Task<bool> TraiterValidationAsync(ValidationRequestDto dto)
    {
        // 1. Récupérer la validation existante
        var validationExistante = await _validationRepo.GetByValideurAndDemandeAsync(dto.ValideurId, dto.DemandeCongeId);
        if (validationExistante == null)
            return false;

        // 2. Mettre à jour la validation existante
        var validationToUpdate = new Validation
        {
            Id = validationExistante.Id,
            DemandeCongeId = validationExistante.DemandeCongeId,
            ValideurId = validationExistante.ValideurId,
            Statut = dto.Statut,
            Commentaire = dto.Commentaire,
            DateValidation = DateTime.UtcNow,
            OrdreValidation = validationExistante.OrdreValidation
        };

        await _validationRepo.UpdateAsync(validationToUpdate);

        // 3. Récupérer la demande et le demandeur
        var demande = await _demandeRepo.GetByIdAsync(dto.DemandeCongeId);
        if (demande == null) return false;

        var demandeur = await _utilisateurRepo.GetByIdAsync(demande.UtilisateurId);
        if (demandeur == null) return false;

        // 4. Traitement si rejeté
        if (dto.Statut == "Rejeté")
        {
            await UpdateDemandeStatutAsync(demande, "Rejeté");
            await EnvoyerEmailAsync(demandeur, "rejetée", dto.Commentaire);
            return true;
        }

        // 5. Traitement si validé - vérifier le rôle du valideur
        var valideur = await _utilisateurRepo.GetByIdAsync(dto.ValideurId);
        if (valideur == null) return false;

        if (valideur.Role == "DOT")
        {
            // Validation finale par DOT
            await UpdateDemandeStatutAsync(demande, "Validé");
            await EnvoyerEmailAsync(demandeur, "validée", null);
        }
        else
        {
            // Passer au DOT pour validation finale
            var dot = await _utilisateurRepo.GetByRoleAsync("DOT");
            if (dot == null)
                throw new InvalidOperationException("Aucun utilisateur DOT trouvé");

            await _validationRepo.CreateAsync(new Validation
            {
                DemandeCongeId = dto.DemandeCongeId,
                ValideurId = dot.Id,
                Statut = "En attente",
                OrdreValidation = validationExistante.OrdreValidation + 1
            });
        }

        return true;
    }

    // Méthodes utilitaires privées
    private async Task UpdateDemandeStatutAsync(DemandeCongeDto demande, string statut)
    {
        var demandeToUpdate = new DemandeCongeDto
        {
            Id = demande.Id,
            DateDebut = demande.DateDebut,
            DateFin = demande.DateFin,
            Motif = demande.Motif,
            Statut = statut,
            UtilisateurId = demande.UtilisateurId,
            DateSoumission = demande.DateSoumission,
            NomUtilisateur = demande.NomUtilisateur,
            EmailUtilisateur = demande.EmailUtilisateur
        };

        await _demandeRepo.UpdateAsync(demandeToUpdate);
    }

    private async Task EnvoyerEmailAsync(Utilisateur demandeur, string action, string? commentaire)
    {
        var sujet = action == "rejetée" ? "Demande de congé rejetée" : "Demande de congé validée";
        var corps = action == "rejetée"
            ? $"Bonjour {demandeur.Nom}, votre demande de congé a été rejetée. Commentaire : {commentaire}"
            : $"Bonjour {demandeur.Nom}, votre demande de congé a été validée définitivement.";

        var mailData = new MailData
        {
            To = demandeur.Email,
            Subject = sujet,
            Body = corps
        };

        await _mailService.SendEmailAsync(mailData);
    }
}