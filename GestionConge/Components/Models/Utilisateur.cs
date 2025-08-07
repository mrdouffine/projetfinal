using System.ComponentModel.DataAnnotations.Schema;

namespace GestionConge.Components.Models;

public class Utilisateur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string MotDePasse { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Ex: Employe, RH, DOT, Admin
    public ICollection<DemandeConge> Demandes { get; set; } = new List<DemandeConge>();

    public int SuperieurId { get; set; }

    public ICollection<Utilisateur>? Subordonnes { get; set; }
}

