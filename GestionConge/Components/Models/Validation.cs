namespace GestionConge.Components.Models;

public class Validation
{
    public int Id { get; set; }
    public int DemandeCongeId { get; set; }
    public int ValideurId { get; set; } // Qui valide ?
    public Utilisateur Valideur { get; set; }
    public string Statut { get; set; } = "En attente"; // "Validé", "Rejeté"
    public string? Commentaire { get; set; }
    public DateTime DateValidation { get; set; } = DateTime.UtcNow;
}