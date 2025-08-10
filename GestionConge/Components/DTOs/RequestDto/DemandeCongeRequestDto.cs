namespace GestionConge.Components.DTOs.RequestDto;

public class DemandeCongeRequestDto
{
    public int UtilisateurId { get; set; }

    public DateTime DateDebut { get; set; }

    public DateTime DateFin { get; set; }

    public string Motif { get; set; }

    //public int? SuperieurId { get; set; }

    //public int? BackupId { get; set; }

    //// Tu peux éventuellement ajouter un champ pour le type de congé si nécessaire
    //public string TypeConge { get; set; }  // ex: "Annuel", "Maladie", etc.
}
