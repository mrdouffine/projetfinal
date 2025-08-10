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
    //public async Task<IEnumerable<PlanningCongeDto>> GetByUtilisateurAsync(int utilisateurId) =>
    //(await _repository.GetAllAsync())
    //    .Where(p => p.UtilisateurId == utilisateurId);
    public async Task<IEnumerable<PlanningCongeDto>> GetByUtilisateurAsync(int utilisateurId) =>
        await _repository.GetByUtilisateurIdAsync(utilisateurId);

    public async Task<bool> VerifierChevauchementAsync(int utilisateurId, DateTime debut, DateTime fin) =>
        await _repository.VerifierChevauchementAsync(utilisateurId, debut, fin);

    public async Task<int> CalculerTotalJoursPlanifiesAsync(int utilisateurId, int annee) =>
        await _repository.CalculerTotalJoursPlanifiesAsync(utilisateurId, annee);

    public async Task<bool> AjouterPlanningAvecVerificationsAsync(PlanningConge planning)
    {
        // ✨ AJOUT : Validation des dates
        if (planning.DateDebut >= planning.DateFin)
            throw new ArgumentException("La date de fin doit être postérieure à la date de début");

        if (planning.DateDebut < DateTime.Today)
            throw new ArgumentException("Impossible de planifier des congés dans le passé");

        // Vérifier chevauchement
        if (await VerifierChevauchementAsync(planning.UtilisateurId, planning.DateDebut, planning.DateFin))
            return false;

        // ✨ AMÉLIORATION : Gestion des congés sur plusieurs années
        var joursNouveau = CalculerJoursOuvrables(planning.DateDebut, planning.DateFin);

        // Vérification par année concernée
        var anneesImpactees = GetAnneesImpactees(planning.DateDebut, planning.DateFin);

        foreach (var annee in anneesImpactees)
        {
            int totalJours = await CalculerTotalJoursPlanifiesAsync(planning.UtilisateurId, annee);
            var joursAnnee = CalculerJoursPourAnnee(planning.DateDebut, planning.DateFin, annee);

            if (totalJours + joursAnnee > 30)
                return false;
        }

        await _repository.CreateAsync(planning);
        return true;
    }

    // Méthode pour calculer les jours de congé pour une année spécifique
    private int CalculerJoursPourAnnee(DateTime debut, DateTime fin, int annee)
    {
        var debutAnnee = new DateTime(annee, 1, 1);
        var finAnnee = new DateTime(annee, 12, 31);

        // Intersection entre la période de congé et l'année
        var debutEffectif = debut > debutAnnee ? debut : debutAnnee;
        var finEffectif = fin < finAnnee ? fin : finAnnee;

        // Si pas d'intersection, retourner 0
        if (debutEffectif > finEffectif)
            return 0;

        return (finEffectif - debutEffectif).Days + 1;
    }

    // Méthodes utilitaires
    private int CalculerJoursOuvrables(DateTime debut, DateTime fin)
    {
        int jours = 0;
        for (var date = debut; date <= fin; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                jours++;
        }
        return jours;
    }

    private List<int> GetAnneesImpactees(DateTime debut, DateTime fin) =>
        Enumerable.Range(debut.Year, fin.Year - debut.Year + 1).ToList();

    public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);
}
