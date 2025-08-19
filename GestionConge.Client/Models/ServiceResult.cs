namespace GestionConge.Client.Models
{
    // Classe générique pour encapsuler les résultats des services
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static ServiceResult<T> Failure(string errorMessage)
        {
            return new ServiceResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }

    // Modèles principaux
    public class PlanningConge
    {
        public int Id { get; set; }
        public int UtilisateurId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public int NombreJours { get; set; }
        public string TypeConge { get; set; } = string.Empty;
        public string? Commentaire { get; set; }
        public DateTime DateCreation { get; set; }

        // Propriétés de navigation
        public string? NomUtilisateur { get; set; }
        public string? PrenomUtilisateur { get; set; }
    }



    public class Rappel
    {
        public int Id { get; set; }
        public int UtilisateurId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime DateEcheance { get; set; }
        public bool EstLu { get; set; }
        public DateTime DateCreation { get; set; }

        // Propriétés de navigation
        public string? NomUtilisateur { get; set; }
        public string? PrenomUtilisateur { get; set; }

    }
    public class NotificationDto {
            public int Id { get; set; }
            public string Titre { get; set; } = "";
            public string Message { get; set; } = "";
            public string Type { get; set; } = "";
            public DateTime DateCreation { get; set; }
            public bool EstLue { get; set; }
            public string Source { get; set; } = "";



        }

    public class Validation
    {
        public int Id { get; set; }
        public int DemandeId { get; set; }
        public int ValidateurId { get; set; }
        public string Statut { get; set; } = string.Empty; // "En attente", "Approuve", "Rejete"
        public string? Commentaire { get; set; }
        public DateTime DateValidation { get; set; }

        // Propriétés de navigation
        public string? NomValidateur { get; set; }
        public string? PrenomValidateur { get; set; }
        public DemandeCongeDto? Demande { get; set; }
    }

    public class UtilisateurAuth
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MotDePasse { get; set; } = string.Empty;
        public string Role { get; set; } = "Employe"; // Employe, DOT, Admin
        public int? ManagerId { get; set; }
        public DateTime DateEmbauche { get; set; }
    }
}

namespace GestionConge.Client.Models.DTOs
{
    public class DemandeCongeDto
    {
        public int Id { get; set; }
        public int UtilisateurId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public int NombreJours { get; set; }
        public string TypeConge { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty; // "En attente", "Approuve", "Rejete"
        public string? Commentaire { get; set; }
        public DateTime DateDemande { get; set; }
        public int? ValidateurId { get; set; }

        // Propriétés de navigation
        public string? NomUtilisateur { get; set; }
        public string? PrenomUtilisateur { get; set; }
        public string? NomValidateur { get; set; }
        public string? PrenomValidateur { get; set; }
    }

    public class DemandeCongeRequestDto
    {
        [Required(ErrorMessage = "La date de début est requise")]
        public DateTime DateDebut { get; set; }

        [Required(ErrorMessage = "La date de fin est requise")]
        public DateTime DateFin { get; set; }

        [Required(ErrorMessage = "Le type de congé est requis")]
        public string TypeConge { get; set; } = string.Empty;

        public string? Commentaire { get; set; }

        public int UtilisateurId { get; set; }
        public int? ValidateurId { get; set; }
    }

    public class UtilisateurDto
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
        public DateTime DateEmbauche { get; set; }

        // Propriétés calculées
        public string NomComplet => $"{Prenom} {Nom}";
        public string? NomManager { get; set; }
        public string? PrenomManager { get; set; }
    }

    public class ValidationRequestDto
    {
        [Required]
        public int DemandeId { get; set; }

        [Required]
        public int ValidateurId { get; set; }

        [Required]
        public string Statut { get; set; } = string.Empty; // "Approuve" ou "Rejete"

        public string Commentaire { get; set; } = string.Empty;
    }

    public class UtilisateurRequestDto
    {
        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        public string MotDePasse { get; set; } = string.Empty;
    }

    // DTOs spécialisés pour les statistiques
    public class StatistiquesCongeDto
    {
        public int TotalDemandesEnAttente { get; set; }
        public int TotalDemandesApprouvees { get; set; }
        public int TotalDemandesRejetees { get; set; }
        public int JoursCongesPris { get; set; }
        public int JoursCongesRestants { get; set; }
        public IEnumerable<DemandeCongeDto> ProchainesAbsences { get; set; } = new List<DemandeCongeDto>();
    }

    public class CalendrierCongeDto
    {
        public DateTime Date { get; set; }
        public List<AbsenceJournaliere> Absences { get; set; } = new();
    }

    public class AbsenceJournaliere
    {
        public int UtilisateurId { get; set; }
        public string NomUtilisateur { get; set; } = string.Empty;
        public string PrenomUtilisateur { get; set; } = string.Empty;
        public string TypeConge { get; set; } = string.Empty;
        public bool EstDebutConge { get; set; }
        public bool EstFinConge { get; set; }
    }

    // Enums
    public static class TypesConge
    {
        public const string CongeAnnuel = "Congé annuel";
        public const string CongeMaladie = "Congé maladie";
        public const string CongeMaternite = "Congé maternité";
        public const string CongePaternite = "Congé paternité";
        public const string CongeSansSolde = "Congé sans solde";
        public const string CongeFormation = "Congé formation";
        public const string RTT = "RTT";

        public static List<string> GetAll()
        {
            return new List<string>
            {
                CongeAnnuel,
                CongeMaladie,
                CongeMaternite,
                CongePaternite,
                CongeSansSolde,
                CongeFormation,
                RTT
            };
        }
    }

    public static class StatutsValidation
    {
        public const string EnAttente = "En attente";
        public const string Valide = "Validé";
        public const string Rejete = "Rejeté";

        public static List<string> GetAll()
        {
            return new List<string> { EnAttente, Valide, Rejete };
        }
    }

    public static class Roles
    {
        public const string Employe = "Employé";
        public const string DOT = "DOT";
        public const string Admin = "Admin";

        public static List<string> GetAll()
        {
            return new List<string> { Employe, DOT, Admin };
        }
    }
}