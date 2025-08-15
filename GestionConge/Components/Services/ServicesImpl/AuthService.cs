using GestionConge.Components.Auth;
using GestionConge.Components.Models;
using GestionConge.Components.Repositories.IRepositories;
using GestionConge.Components.Services.IServices;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

    // =====================
    // LOGIN
    // =====================
    public async Task<AuthResponse?> LoginAsync(LoginRequest req)
    {
        // Cherche par email, sinon par nom d’utilisateur
        var user = await _users.GetByEmailAsync(req.UserNameOrEmail)
                   ?? await _users.GetByUserNameAsync(req.UserNameOrEmail);
        if (user is null) return null;

        // Vérif du mot de passe (BCrypt)
        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.MotDePasse))
            return null;

        // Générer tokens
        var (accessToken, accessExpUtc) = GenerateAccessToken(user);
        var (refreshToken, refreshExpUtc) = GenerateRefreshToken();

        // Persister le refresh token
        await _users.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshExpUtc);

        return new AuthResponse
        {
            UserId = user.Id,
            UserName = user.Nom,
            Email = user.Email,
            Role = user.Role,
            AccessToken = accessToken,
            AccessTokenExpires = accessExpUtc,
            RefreshToken = refreshToken,
            RefreshTokenExpires = refreshExpUtc
        };
    }

    // =====================
    // REGISTER
    // =====================
    public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
    {
        if (await _users.GetByEmailAsync(req.Email) is not null)
            throw new Exception("Email déjà utilisé.");

        if (!string.IsNullOrWhiteSpace(req.Nom) &&
            await _users.GetByUserNameAsync(req.Nom) is not null)
            throw new Exception("Nom d’utilisateur déjà utilisé.");

        var user = new UtilisateurAuth
        {
            Nom = string.IsNullOrWhiteSpace(req.Nom) ? req.Email : req.Nom,
            Email = req.Email,
            Role = string.IsNullOrWhiteSpace(req.Role) ? "Employe" : req.Role,
            MotDePasse = BCrypt.Net.BCrypt.HashPassword(req.MotDePasse)
        };

        user.Id = await _users.CreateAsync(user);

        var (accessToken, accessExpUtc) = GenerateAccessToken(user);
        var (refreshToken, refreshExpUtc) = GenerateRefreshToken();
        await _users.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshExpUtc);

        return new AuthResponse
        {
            UserId = user.Id,
            UserName = user.Nom,
            Email = user.Email,
            Role = user.Role,
            AccessToken = accessToken,
            AccessTokenExpires = accessExpUtc,
            RefreshToken = refreshToken,
            RefreshTokenExpires = refreshExpUtc
        };
    }

    // =====================
    // REFRESH TOKEN
    // =====================
    public async Task<AuthResponse?> RefreshAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return null;

        var user = await _users.GetByRefreshTokenAsync(refreshToken);
        if (user is null) return null;

        // (Optionnel) vérifier l'expiration côté code si le repo ne filtre pas déjà
        // if (user.RefreshTokenExpiryDate < DateTime.UtcNow) return null;


        var (newAccess, newAccessExpUtc) = GenerateAccessToken(user);
        var (newRefresh, newRefreshExpUtc) = GenerateRefreshToken();

        await _users.UpdateRefreshTokenAsync(user.Id, newRefresh, newRefreshExpUtc);

        return new AuthResponse
        {
            UserId = user.Id,
            UserName = user.Nom,
            Email = user.Email,
            Role = user.Role,
            AccessToken = newAccess,
            AccessTokenExpires = newAccessExpUtc,
            RefreshToken = newRefresh,
            RefreshTokenExpires = newRefreshExpUtc
        };
    }

    // =====================
    // LOGOUT
    // =====================
    public async Task<bool> LogoutAsync(int userId)
    {
        // On invalide le refresh token (NULL/empty + date nulle)
        await _users.UpdateRefreshTokenAsync(userId, null, null);
        return true;
    }

    // =====================
    // Helpers
    // =====================
    private (string token, DateTime expiresUtc) GenerateAccessToken(UtilisateurAuth user)
    {
        var issuer = _cfg["JwtSettings:Issuer"]!;
        var audience = _cfg["JwtSettings:Audience"]!;
        var secret = _cfg["JwtSettings:SecretKey"]!;
        // par défaut 60 minutes si non configuré
        var accessMinutes = int.TryParse(_cfg["JwtSettings:AccessTokenMinutes"], out var m) ? m : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Nom),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(accessMinutes),
            signingCredentials: creds
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return (token, jwt.ValidTo);
    }

    private (string token, DateTime expiresUtc) GenerateRefreshToken()
    {
        var days = int.TryParse(_cfg["JwtSettings:RefreshTokenDays"], out var d) ? d : 7;

        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);

        var token = Convert.ToBase64String(bytes);
        return (token, DateTime.UtcNow.AddDays(days));
    }
}
