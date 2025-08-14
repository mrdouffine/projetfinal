namespace GestionConge.Components.Auth;

public class AuthResponse
{
    public string Token { get; set; } = "";
    public DateTime ExpiresAtUtc { get; set; }
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
}
