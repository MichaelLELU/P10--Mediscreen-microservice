using AuthenticationService.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Api.Data;

public static class IdentityDataSeeder
{
    public static async Task SeedDemoUserAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        using IServiceScope scope =
            serviceProvider.CreateScope();

        UserManager<ApplicationUser> userManager =
            scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

        string email =
            configuration["DemoUser:Email"]
            ?? throw new InvalidOperationException(
                "L'adresse e-mail de l'utilisateur de démonstration est manquante.");

        string password =
            configuration["DemoUser:Password"]
            ?? throw new InvalidOperationException(
                "Le mot de passe de l'utilisateur de démonstration est manquant.");

            ApplicationUser? existingUser =
            await userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return;
        }

        ApplicationUser demoUser = new()
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        IdentityResult result =
            await userManager.CreateAsync(demoUser, password);

        if (!result.Succeeded)
        {
            string errors = string.Join(
                ", ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Impossible de créer l'utilisateur de démonstration : {errors}");
        }
    }
}