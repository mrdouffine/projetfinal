namespace GestionConge.Components.Auth;

public class RegisterRequest
{
    public string Nom { get; set; } = "";   // optionnel si tu ne veux que l'email
    public string Email { get; set; } = "";
    public string MotDePasse { get; set; } = "";
    public string? Role { get; set; }             // par défaut "Employe"
}
