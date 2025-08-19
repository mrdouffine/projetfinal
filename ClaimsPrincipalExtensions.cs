using System;
using System.Security.Claims;

namespace GestionConge.Client.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Récupère l'identifiant utilisateur à partir du claim "sub" ou "nameidentifier".
        /// </summary>
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            if (user == null) return null;
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("sub");
            if (idClaim != null && int.TryParse(idClaim.Value, out var id))
                return id;
            return null;
        }
    }
}
@using GestionConge.Client.Extensions
