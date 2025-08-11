namespace GestionConge.Client.Models
{
    public class UtilisateurDto
    {
        public string Nom { get; set; }

        public string Email { get; set; }

        public string MotDePasse { get; set; }

        public string Role { get; set; } // Exemple : "Employé", "Admin", etc.

        public int? SuperieurId { get; set; } // ID du supérieur hiérarchique, s’il y en a
    }
}
