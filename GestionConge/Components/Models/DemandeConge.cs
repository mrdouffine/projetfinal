namespace GestionConge.Components.Models;

public class DemandeConge
{
    public int Id { get; set; }
    public int UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string Motif { get; set; } = string.Empty;
    public string Statut { get; set; } = "En attente"; // ou "Validé", "Rejeté"
    public DateTime DateSoumission { get; set; } = DateTime.UtcNow;
    public Validation Validation { get; set; }

}
