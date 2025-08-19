using GestionConge.Client.Models;
using GestionConge.Client.Models.DTOs;
using System.Security.Claims;
using DemandeCongeDto = GestionConge.Client.Models.DTOs.DemandeCongeDto;

namespace GestionConge.Client.Extensions
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Extension pour gérer facilement les résultats de services avec notifications
        /// </summary>
        //public static async Task HandleServiceResultAsync<T>(
        //    this ServiceResult<T> result,
        //    ISnackbar snackbar,
        //    string? successMessage = null,
        //    Action<T>? onSuccess = null)
        //{
        //    if (result.IsSuccess)
        //    {
        //        if (!string.IsNullOrEmpty(successMessage))
        //        {
        //            snackbar.Add(successMessage, Severity.Success);
        //        }

        //        if (onSuccess != null && result.Data != null)
        //        {
        //            onSuccess(result.Data);
        //        }
        //    }
        //    else
        //    {
        //        snackbar.Add(result.ErrorMessage, Severity.Error);
        //    }
        //}

        /// <summary>
        /// Extension pour convertir ServiceResult en bool avec notification d'erreur
        /// </summary>
        public static bool HandleWithNotification<T>(this ServiceResult<T> result, ISnackbar snackbar)
        {
            if (!result.IsSuccess)
            {
                snackbar.Add(result.ErrorMessage, Severity.Error);
            }
            return result.IsSuccess;
        }
    }

    public static class DateExtensions
    {
        /// <summary>
        /// Calcule le nombre de jours ouvrables entre deux dates
        /// </summary>
        public static int CalculerJoursOuvrables(this DateTime dateDebut, DateTime dateFin)
        {
            var jours = 0;
            var current = dateDebut.Date;

            while (current <= dateFin.Date)
            {
                if (current.DayOfWeek != DayOfWeek.Saturday &&
                    current.DayOfWeek != DayOfWeek.Sunday)
                {
                    jours++;
                }
                current = current.AddDays(1);
            }

            return jours;
        }

        /// <summary>
        /// Vérifie si une date est un week-end
        /// </summary>
        public static bool EstWeekEnd(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>
        /// Formate une date pour l'affichage français
        /// </summary>
        public static string ToFrenchDateString(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Formate une période pour l'affichage
        /// </summary>
        public static string ToPeriodString(this DateTime dateDebut, DateTime dateFin)
        {
            if (dateDebut.Date == dateFin.Date)
            {
                return dateDebut.ToFrenchDateString();
            }
            return $"{dateDebut.ToFrenchDateString()} - {dateFin.ToFrenchDateString()}";
        }
    }

    public static class CongeExtensions
    {
        /// <summary>
        /// Obtient la couleur associée à un statut de demande
        /// </summary>
        public static Color GetStatutColor(this string statut)
        {
            return statut switch
            {
                StatutsValidation.EnAttente => Color.Warning,
                StatutsValidation.Valide => Color.Success,
                StatutsValidation.Rejete => Color.Error,
                _ => Color.Default
            };
        }

        /// <summary>
        /// Obtient l'icône associée à un statut de demande
        /// </summary>
        public static string GetStatutIcon(this string statut)
        {
            return statut switch
            {
                StatutsValidation.EnAttente => Icons.Material.Filled.HourglassEmpty,
                StatutsValidation.Valide => Icons.Material.Filled.CheckCircle,
                StatutsValidation.Rejete => Icons.Material.Filled.Cancel,
                _ => Icons.Material.Filled.Help
            };
        }

        /// <summary>
        /// Obtient la couleur associée à un type de congé
        /// </summary>
        public static Color GetTypeCongeColor(this string typeConge)
        {
            return typeConge switch
            {
                TypesConge.CongeAnnuel => Color.Primary,
                TypesConge.CongeMaladie => Color.Error,
                TypesConge.CongeMaternite => Color.Secondary,
                TypesConge.CongePaternite => Color.Info,
                TypesConge.CongeSansSolde => Color.Warning,
                TypesConge.CongeFormation => Color.Success,
                TypesConge.RTT => Color.Tertiary,
                _ => Color.Default
            };
        }

        /// <summary>
        /// Vérifie si une demande peut être modifiée
        /// </summary>
        public static bool PeutEtreModifiee(this DemandeCongeDto demande)
        {
            return demande.Statut == StatutsValidation.EnAttente &&
                   demande.DateDebut > DateTime.Now.AddDays(1);
        }

        /// <summary>
        /// Vérifie si une demande peut être annulée
        /// </summary>
        public static bool PeutEtreAnnulee(this DemandeCongeDto demande)
        {
            return demande.Statut != StatutsValidation.Rejete &&
                   demande.DateDebut > DateTime.Now;
        }
    }

    //public static class ClaimsPrincipalExtensions
    //{
    //    /// <summary>
    //    /// Récupère l'identifiant utilisateur à partir du claim "sub" ou "nameidentifier".
    //    /// </summary>
    //    public static int? GetUserId(this ClaimsPrincipal user)
    //    {
    //        if (user == null) return null;
    //        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("sub");
    //        if (idClaim != null && int.TryParse(idClaim.Value, out var id))
    //            return id;
    //        return null;
    //    }
    //}

    public static class ClaimsExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        public static string? GetUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }

        public static string? GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static string? GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }
    }

    public static class ServiceResultExtensions
    {
        public static async Task HandleServiceResultAsync<T>(this ServiceResult<T> result,
            ISnackbar snackbar,
            string? successMessage = null,
            Action<T>? onSuccess = null)
        {
            if (result.IsSuccess)
            {
                if (!string.IsNullOrEmpty(successMessage))
                {
                    snackbar.Add(successMessage, Severity.Success);
                }

                if (onSuccess != null && result.Data != null)
                {
                    onSuccess(result.Data);
                }
            }
            else
            {
                snackbar.Add(result.ErrorMessage ?? "Une erreur s'est produite", Severity.Error);
            }
        }

        public static bool HandleWithNotification<T>(this ServiceResult<T> result,
            ISnackbar snackbar,
            string? successMessage = null)
        {
            if (result.IsSuccess)
            {
                if (!string.IsNullOrEmpty(successMessage))
                {
                    snackbar.Add(successMessage, Severity.Success);
                }
                return true;
            }
            else
            {
                snackbar.Add(result.ErrorMessage ?? "Une erreur s'est produite", Severity.Error);
                return false;
            }
        }
    }
}

    //public static class UserExtensions
    //{
    //    /// <summary>
    //    ///