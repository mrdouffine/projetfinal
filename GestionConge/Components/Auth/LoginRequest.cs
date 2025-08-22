namespace GestionConge.Components.Auth;

public class LoginRequest
{
    public string Email { get; set; } = ""; // on autorise email OU username
    public string MotDePasse { get; set; } = "";
}
