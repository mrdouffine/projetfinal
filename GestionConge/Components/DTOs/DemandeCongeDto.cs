namespace GestionConge.Components.DTOs;

public class DemandeCongeDto
{
    public int Id { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string Motif { get; set; }
    public string Statut { get; set; }
    public DateTime DateSoumission { get; set; }

    // Infos de l'utilisateur
    public int UtilisateurId { get; set; }
    public string NomUtilisateur { get; set; }
    public string EmailUtilisateur { get; set; }

    // Infos de validation
    public string? StatutValidation { get; set; }
    public string? CommentaireValidation { get; set; }
    public DateTime? DateValidation { get; set; }
}


