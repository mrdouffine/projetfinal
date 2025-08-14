namespace GestionConge.Components.Auth;

public class LoginRequest
{
    public string UserNameOrEmail { get; set; } = ""; // on autorise email OU username
    public string Password { get; set; } = "";
}
