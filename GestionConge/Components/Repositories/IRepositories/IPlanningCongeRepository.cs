namespace GestionConge.Components.Repositories.IRepositories;

using GestionConge.Components.DTOs;
using GestionConge.Components.Models;

public interface IPlanningCongeRepository
{
    Task<IEnumerable<PlanningCongeDto>> GetAllAsync();
    Task<PlanningCongeDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(PlanningConge planning);
    Task<bool> UpdateAsync(PlanningConge planning);
    Task<bool> VerifierChevauchementAsync(int utilisateurId, DateTime debut, DateTime fin);
    Task<int> CalculerTotalJoursPlanifiesAsync(int utilisateurId, int annee);

    Task<bool> DeleteAsync(int id);
}
