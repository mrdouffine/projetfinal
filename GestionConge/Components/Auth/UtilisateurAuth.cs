namespace GestionConge.Components.Auth;

public class UtilisateurAuth // ou réutilise ton modèle Utilisateur
{
    public int Id { get; set; }
    public string Nom { get; set; } = "";          // équiv. UserName
    public string Email { get; set; } = "";
    public string MotDePasse { get; set; } = "";   // contiendra le HASH
    public string Role { get; set; } = "Employe";
}
