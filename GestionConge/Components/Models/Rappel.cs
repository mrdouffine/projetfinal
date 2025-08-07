namespace GestionConge.Components.Models;

public class Rappel
{
    public int Id { get; set; }
    public string Message { get; set; }
    public DateTime DateRappel { get; set; }
    public int UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; }
}

