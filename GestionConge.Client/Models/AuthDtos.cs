namespace GestionConge.Client.Models
{
    public class AuthDtos
    {
        public class LoginDto
        {
            public string Email { get; set; } = string.Empty;
            public string MotDePasse { get; set; } = string.Empty;
        }
        public class RegisterDto
        {
            public string Nom { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string MotDePasse { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;  // Exemple : "Employé", "Admin", etc.
            //public int? SuperieurId { get; set; }  // ID du supérieur hiérarchique, s’il y en a
        }
        public class ResetPasswordDto
        {
            public string Email { get; set; } = string.Empty;
            public string NouveauMotDePasse { get; set; } = string.Empty;
        }
    }
}
