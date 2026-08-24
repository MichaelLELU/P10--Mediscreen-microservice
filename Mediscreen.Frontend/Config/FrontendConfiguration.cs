using Mediscreen.Frontend.Handlers;
using Mediscreen.Frontend.Services;
using Mediscreen.Frontend.Services.Interfaces;

using FrontendAuthenticationService =
    Mediscreen.Frontend.Services.Interfaces.IAuthenticationService;

namespace Mediscreen.Frontend.Configurations;

public static class FrontendConfiguration
{
    public static IServiceCollection AddFrontendServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string gatewayBaseUrl =
            configuration["Gateway:BaseUrl"]
            ?? throw new InvalidOperationException(
                "L'adresse de la Gateway est manquante.");

        services.AddHttpContextAccessor();

        services.AddTransient<JwtAuthorizationHandler>();

        services.AddHttpClient("Gateway", client =>
        {
            client.BaseAddress = new Uri(gatewayBaseUrl);
        })
        .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddScoped<
            IPatientService,
            PatientService>();

        services.AddScoped<
            INoteService,
            NoteService>();

        services.AddScoped<
            FrontendAuthenticationService,
            AuthenticationService>();

        return services;
    }
}