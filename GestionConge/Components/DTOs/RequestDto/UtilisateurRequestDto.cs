namespace GestionConge.Components.DTOs.RequestDto
{
    public class UtilisateurRequestDto
    {
        public string Nom { get; set; }

        public string Email { get; set; }

        public string MotDePasse { get; set; }

        public string Role { get; set; } // Exemple : "Employé", "Admin", etc.

        public int? SuperieurId { get; set; } // ID du supérieur hiérarchique, s’il y en a

        //public int? BackupId { get; set; } // ID du remplaçant en cas d’absence

        //public bool EstActif { get; set; } = true; // Optionnel : utilisateur actif ou non
    }

}
