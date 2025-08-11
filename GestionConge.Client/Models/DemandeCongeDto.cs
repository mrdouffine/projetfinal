namespace GestionConge.Client.Models
{
    public class DemandeCongeDto
    {
        public int Id { get; set; }
        public string TypeConge { get; set; } = string.Empty;
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }

        //Motif
        public string Motif { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public string Commentaire { get; set; } = string.Empty;
        public int UtilisateurId { get; set; }
        public UtilisateurDto? Utilisateur { get; set; }
        public ValidationDto? Validation { get; set; }
        public RappelDto? Rappel { get; set; }
    }
}
