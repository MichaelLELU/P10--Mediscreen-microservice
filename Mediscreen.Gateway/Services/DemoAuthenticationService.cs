using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mediscreen.Gateway.Models;
using Mediscreen.Gateway.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Mediscreen.Gateway.Services;

public class DemoAuthenticationService : IAuthenticationService
{
    private const string DemoUsername = "admin";
    private const string DemoPassword = "Admin123!";

    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<DemoUser> _passwordHasher;
    private readonly DemoUser _demoUser;

    public DemoAuthenticationService(IConfiguration configuration)
    {
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<DemoUser>();

        _demoUser = new DemoUser
        {
            Username = DemoUsername
        };

        _demoUser.PasswordHash =
            _passwordHasher.HashPassword(_demoUser, DemoPassword);
    }

    public LoginResponse? Login(LoginRequest request)
    {
        if (!string.Equals(
                request.Username,
                _demoUser.Username,
                StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        PasswordVerificationResult verificationResult =
            _passwordHasher.VerifyHashedPassword(
                _demoUser,
                _demoUser.PasswordHash,
                request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return GenerateToken(_demoUser);
    }

    private LoginResponse GenerateToken(DemoUser user)
    {
        string jwtKey =
            _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "La clé JWT est manquante.");

        string jwtIssuer =
            _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "L'émetteur du JWT est manquant.");

        string jwtAudience =
            _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "L'audience du JWT est manquante.");

        int expirationMinutes =
            _configuration.GetValue<int>("Jwt:ExpirationMinutes");

        DateTime expiration =
            DateTime.UtcNow.AddMinutes(expirationMinutes);

        Claim[] claims =
        [
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Username),

            new Claim(
                ClaimTypes.Name,
                user.Username),

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
            expires: expiration,
            signingCredentials: credentials);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler()
                .WriteToken(token),

            Expiration = expiration
        };
    }
}