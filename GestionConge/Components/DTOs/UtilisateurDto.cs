namespace GestionConge.Components.DTOs;

public class UtilisateurDto
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Email { get; set; }
    public string? MotDePasse { get; set; }
    public string Role { get; set; }
    //superieur
    public int? SuperieurId { get; set; } // ID du supérieur hiérarchique, s’il y en a
}
