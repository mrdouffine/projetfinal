namespace GestionConge.Client.Models;

public class UserSession
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public string Nom { get; set; } = default!;
    public string Role { get; set; } = default!;
}
