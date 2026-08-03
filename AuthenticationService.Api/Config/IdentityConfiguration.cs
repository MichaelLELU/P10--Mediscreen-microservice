using AuthenticationService.Api.Data;
using AuthenticationService.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Api.Config;

public static class IdentityConfiguration
{
    public static IServiceCollection AddIdentityConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString =
            configuration.GetConnectionString("AuthenticationDatabase")
            ?? throw new InvalidOperationException(
                "La chaîne de connexion AuthenticationDatabase est manquante.");

        services.AddDbContext<AuthenticationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddEntityFrameworkStores<AuthenticationDbContext>();

        return services;
    }
}