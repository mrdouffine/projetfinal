using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GestionConge.Client.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILogger<CustomAuthStateProvider> _logger;
        private readonly ILocalStorageService _localStorage;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        private const string TokenKey = "authToken";

        public CustomAuthStateProvider(ILogger<CustomAuthStateProvider> logger, ILocalStorageService localStorage)
        {
            _logger = logger;
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string? token = null;

            try
            {
                token = await _localStorage.GetItemAsync<string>(TokenKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Impossible de lire le token depuis localStorage");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                var user = GetClaimsFromToken(token);
                _currentUser = user;
            }

            return new AuthenticationState(_currentUser);
        }

        public async Task NotifyUserAuthenticationAsync(string token)
        {
            await _localStorage.SetItemAsync(TokenKey, token);

            var authenticatedUser = GetClaimsFromToken(token);
            _currentUser = authenticatedUser;

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(authenticatedUser)));
        }

        public async Task NotifyUserLogoutAsync()
        {
            await _localStorage.RemoveItemAsync(TokenKey);

            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            _currentUser = anonymousUser;

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
        }

        private ClaimsPrincipal GetClaimsFromToken(string token)
        {
            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();

                if (!jwtHandler.CanReadToken(token))
                {
                    _logger.LogWarning("Token JWT invalide");
                    return new ClaimsPrincipal(new ClaimsIdentity());
                }

                var jwtToken = jwtHandler.ReadJwtToken(token);
                var claims = jwtToken.Claims.ToList();

                // Ajoute ClaimTypes.Name si absent
                if (!claims.Any(c => c.Type == ClaimTypes.Name))
                {
                    var emailClaim = claims.FirstOrDefault(c => c.Type == "email" || c.Type == ClaimTypes.Email);
                    if (emailClaim != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Name, emailClaim.Value));
                    }
                }

                var identity = new ClaimsIdentity(claims, "jwt");
                return new ClaimsPrincipal(identity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du parsing du token JWT");
                return new ClaimsPrincipal(new ClaimsIdentity());
            }
        }
    }
}
