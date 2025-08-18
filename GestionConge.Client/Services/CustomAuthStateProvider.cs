namespace GestionConge.Client.Services;

using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;


    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILogger<CustomAuthStateProvider> _logger;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public CustomAuthStateProvider(ILogger<CustomAuthStateProvider> logger)
        {
            _logger = logger;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }

        public Task NotifyUserAuthenticationAsync(string token)
        {
            var authenticatedUser = GetClaimsFromToken(token);
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            _currentUser = authenticatedUser;
            NotifyAuthenticationStateChanged(authState);

            return authState;
        }

        public Task NotifyUserLogoutAsync()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));

            _currentUser = anonymousUser;
            NotifyAuthenticationStateChanged(authState);

            return authState;
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

                // Ajouter les claims standard si ils n'existent pas
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
