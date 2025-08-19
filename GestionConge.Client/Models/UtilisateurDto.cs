namespace GestionConge.Client.Models
{
    public class UtilisateurDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string MotDePasse { get; set; } = string.Empty;

        public string Role { get; set; } // Exemple : "Employé", "Admin", etc.
        //Avatar
        public string Avatar { get; set; } = string.Empty; // URL de l'avatar de l'utilisateur

        public int? SuperieurId { get; set; } // ID du supérieur hiérarchique, s’il y en a
    }
}
