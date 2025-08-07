namespace GestionConge.Components.Models;

public class PlanningConge
{
    public int Id { get; set; }
    public int UtilisateurId { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string Motif { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}


