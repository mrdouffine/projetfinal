namespace GestionConge.Components.DTOs;

public class ValidationDto
{
    public int Id { get; set; }
    public int DemandeCongeId { get; set; }

    public int ValideurId { get; set; }
    public string NomValideur { get; set; }
    public string EmailValideur { get; set; }

    public string Statut { get; set; }
    public string? Commentaire { get; set; }
    public DateTime DateValidation { get; set; }
}
