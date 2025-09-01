namespace GestionConge.Client.Models
{
    public class DemandeCongeRequestDto
    {
        public int UtilisateurId { get; set; }

        public DateTime DateDebut { get; set; }

        public DateTime DateFin { get; set; }

        public string Motif { get; set; }
        
    }
}
