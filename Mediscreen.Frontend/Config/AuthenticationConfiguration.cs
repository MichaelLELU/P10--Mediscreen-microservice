using Microsoft.AspNetCore.Authentication.Cookies;

namespace Mediscreen.Frontend.Configurations;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddFrontendAuthentication(
        this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();

        services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.IdleTimeout = TimeSpan.FromMinutes(30);
        });

        services
            .AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.AccessDeniedPath = "/Auth/Login";
                options.ExpireTimeSpan =
                    TimeSpan.FromMinutes(30);

                options.SlidingExpiration = false;
            });

        return services;
    }
}