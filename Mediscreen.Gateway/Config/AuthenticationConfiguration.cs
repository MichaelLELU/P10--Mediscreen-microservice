using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Mediscreen.Gateway.Config;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string jwtKey =
            configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "La clé JWT est manquante.");

        string jwtIssuer =
            configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "L'émetteur du JWT est manquant.");

        string jwtAudience =
            configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "L'audience du JWT est manquante.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,

                        ValidateAudience = true,
                        ValidAudience = jwtAudience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtKey)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
            });

        services.AddAuthorization();

        return services;
    }
}