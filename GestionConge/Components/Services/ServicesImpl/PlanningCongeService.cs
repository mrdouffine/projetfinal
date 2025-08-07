namespace GestionConge.Components.Services.ServicesImpl;

using GestionConge.Components.DTOs;
using GestionConge.Components.Models;
using GestionConge.Components.Repositories.IRepositories;
using GestionConge.Components.Services.IServices;

public class PlanningCongeService : IPlanningCongeService
{
    private readonly IPlanningCongeRepository _repository;

    public PlanningCongeService(IPlanningCongeRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<PlanningCongeDto>> GetAllAsync() => _repository.GetAllAsync();
    public Task<PlanningCongeDto?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    public Task<int> CreateAsync(PlanningConge planning) => _repository.CreateAsync(planning);
    public Task<bool> UpdateAsync(PlanningConge planning) => _repository.UpdateAsync(planning);
    public async Task<IEnumerable<PlanningCongeDto>> GetByUtilisateurAsync(int utilisateurId) =>
    (await _repository.GetAllAsync())
        .Where(p => p.UtilisateurId == utilisateurId);

    public async Task<bool> VerifierChevauchementAsync(int utilisateurId, DateTime debut, DateTime fin) =>
        await _repository.VerifierChevauchementAsync(utilisateurId, debut, fin);

    public async Task<int> CalculerTotalJoursPlanifiesAsync(int utilisateurId, int annee) =>
        await _repository.CalculerTotalJoursPlanifiesAsync(utilisateurId, annee);

    public async Task<bool> AjouterPlanningAvecVerificationsAsync(PlanningConge planning)
    {
        // Vérifier chevauchement
        if (await VerifierChevauchementAsync(planning.UtilisateurId, planning.DateDebut, planning.DateFin))
            return false;

        // Calcul total jours déjà planifiés
        int annee = planning.DateDebut.Year;
        int totalJours = await CalculerTotalJoursPlanifiesAsync(planning.UtilisateurId, annee);

        // Jours du nouveau planning
        int joursNouveau = (planning.DateFin - planning.DateDebut).Days + 1;
        if (totalJours + joursNouveau > 30)
            return false;

        await _repository.CreateAsync(planning);
        return true;
    }

    public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
}
