using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationService.Api.Models;
using AuthenticationService.Api.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Api.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public LoginResponse CreateToken(ApplicationUser user)
    {
        string jwtKey =
            configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "La clé JWT est manquante.");

        string jwtIssuer =
            configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "L'émetteur JWT est manquant.");

        string jwtAudience =
            configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "L'audience JWT est manquante.");

        int expirationMinutes =
            configuration.GetValue<int>("Jwt:ExpirationMinutes");

        DateTime expiresAt =
            DateTime.UtcNow.AddMinutes(expirationMinutes);

        List<Claim> claims =
        [
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id),

            new Claim(
                JwtRegisteredClaimNames.Email,
                user.Email ?? string.Empty),

            new Claim(
                ClaimTypes.Name,
                user.UserName ?? string.Empty),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        ];

        SymmetricSecurityKey securityKey =
            new(Encoding.UTF8.GetBytes(jwtKey));

        SigningCredentials credentials =
            new(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler()
                .WriteToken(token),

            ExpiresAt = expiresAt,
            Email = user.Email ?? string.Empty
        };
    }
}