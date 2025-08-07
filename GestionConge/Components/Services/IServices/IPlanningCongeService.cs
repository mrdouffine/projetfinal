namespace GestionConge.Components.Services.IServices;

using GestionConge.Components.DTOs;
using GestionConge.Components.Models;
public interface IPlanningCongeService
{
    Task<IEnumerable<PlanningCongeDto>> GetAllAsync();
    Task<PlanningCongeDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(PlanningConge planning);
    Task<bool> UpdateAsync(PlanningConge planning);
    Task<IEnumerable<PlanningCongeDto>> GetByUtilisateurAsync(int utilisateurId);
    Task<bool> AjouterPlanningAvecVerificationsAsync(PlanningConge planning);
    Task<int> CalculerTotalJoursPlanifiesAsync(int utilisateurId, int annee);
    Task<bool> VerifierChevauchementAsync(int utilisateurId, DateTime debut, DateTime fin);

    Task<bool> DeleteAsync(int id);
}
