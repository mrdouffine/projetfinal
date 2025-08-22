using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace GestionConge.Client.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILogger<CustomAuthStateProvider> _logger;
        private readonly IJSRuntime _jsRuntime;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        private const string TokenKey = "authToken";

        public CustomAuthStateProvider(ILogger<CustomAuthStateProvider> logger, IJSRuntime jsRuntime)
        {
            _logger = logger;
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string? token = null;

            try
            {
                // ⚠️ Peut échouer pendant le prerendering → on capture
                token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
            }
            catch (InvalidOperationException)
            {
                // On est en mode prerendering (pas encore de JS dispo)
                _logger.LogWarning("JSRuntime indisponible (prerendering). Retour utilisateur anonyme.");
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
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);

            var authenticatedUser = GetClaimsFromToken(token);
            _currentUser = authenticatedUser;

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(authenticatedUser)));
        }

        public async Task NotifyUserLogoutAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);

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
