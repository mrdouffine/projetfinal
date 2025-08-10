namespace CongesApp.Models;
public class DemandeCongeModel
{
    public int Id { get; set; }
    public DateTime DateDebut { get; set; } = DateTime.Today;
    public DateTime DateFin { get; set; } = DateTime.Today;
    public string Motif { get; set; } = string.Empty;
    public string Statut { get; set; } = "EnAttente";
    public int RequesterId { get; set; }
}
