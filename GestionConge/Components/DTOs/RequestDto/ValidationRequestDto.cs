namespace GestionConge.Components.DTOs.RequestDto;

public class ValidationRequestDto
{
    public int DemandeCongeId { get; set; }
    public int ValideurId { get; set; }
    public string Statut { get; set; } // "Validé" ou "Rejeté"
    public string? Commentaire { get; set; }
}
