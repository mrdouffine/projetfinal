namespace GestionConge.Client.Models
{
    public class DemandeCongeDto
    {
        public int Id { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string Motif { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public DateTime DateSoumission { get; set; }

        // Infos de l'utilisateur
        public int UtilisateurId { get; set; }
        public string NomUtilisateur { get; set; } = string.Empty;
        public string EmailUtilisateur { get; set; } = string.Empty;

        public string? TypeConge { get; set; }
        // Infos de validation
        public string? StatutValidation { get; set; }
        public string? CommentaireValidation { get; set; }
        public DateTime? DateValidation { get; set; }
    }
}
