using GestionConge.Components.Auth;
using GestionConge.Components.Repositories.IRepositories;
using GestionConge.Components.Services.IServices;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestionConge.Components.Services.ServicesImpl;


public class AuthService : IAuthService
{
    private readonly IUtilisateurRepository _users;
    private readonly IConfiguration _cfg;

    public AuthService(IUtilisateurRepository users, IConfiguration cfg)
    {
        _users = users;
        _cfg = cfg;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest req)
    {
        // On cherche d'abord par email, sinon par nom
        var user = await _users.GetByEmailAsync(req.UserNameOrEmail)
                   ?? await _users.GetByUserNameAsync(req.UserNameOrEmail);
        if (user is null) return null;

        // Vérif BCrypt
        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.MotDePasse))
            return null;

        return GenerateToken(user);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        // Vérifs basiques de doublons
        if (await _users.GetByEmailAsync(req.Email) is not null)
            throw new Exception("Email déjà utilisé");
        if (!string.IsNullOrWhiteSpace(req.Nom) &&
            await _users.GetByUserNameAsync(req.Nom) is not null)
            throw new Exception("Nom d'utilisateur déjà utilisé");

        var user = new UtilisateurAuth
        {
            Nom = string.IsNullOrWhiteSpace(req.Nom) ? req.Email : req.Nom,
            Email = req.Email,
            Role = string.IsNullOrWhiteSpace(req.Role) ? "Employe" : req.Role
        };

        // Hash du mot de passe (BCrypt)
        user.MotDePasse = BCrypt.Net.BCrypt.HashPassword(req.MotDePasse);

        user.Id = await _users.CreateAsync(user);

        return GenerateToken(user);
    }

    private AuthResponse GenerateToken(UtilisateurAuth user)
    {
        var issuer = _cfg["JwtSettings:Issuer"]!;
        var audience = _cfg["JwtSettings:Audience"]!;
        var secret = _cfg["JwtSettings:SecretKey"]!;
        var expiresHours = int.Parse(_cfg["JwtSettings:ExpiresHours"] ?? "8");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Nom),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiresHours),
            signingCredentials: creds
        );

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = token.ValidTo,
            UserName = user.Nom,
            Email = user.Email,
            Role = user.Role
        };
    }
}
