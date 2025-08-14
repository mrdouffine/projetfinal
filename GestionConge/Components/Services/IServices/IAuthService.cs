using GestionConge.Components.Auth;

namespace GestionConge.Components.Services.IServices;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest req);
    Task<AuthResponse> RegisterAsync(RegisterRequest req);
}
