namespace GestionConge.Components.DTOs;

public class PlanningCongeDto
{
    public int Id { get; set; }
    public int UtilisateurId { get; set; }
    public string NomUtilisateur { get; set; }
    public DateTime DateDebut { get; set; }
    public DateTime DateFin { get; set; }
    public string Motif { get; set; }
    public DateTime DateCreation { get; set; }
}
