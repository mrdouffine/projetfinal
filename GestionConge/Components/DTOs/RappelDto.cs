namespace GestionConge.Components.DTOs;

public class RappelDto
{
    public int Id { get; set; }
    public string Message { get; set; }
    public DateTime DateRappel { get; set; }

    public int UtilisateurId { get; set; }
    public string NomUtilisateur { get; set; }
    public string EmailUtilisateur { get; set; }
}
