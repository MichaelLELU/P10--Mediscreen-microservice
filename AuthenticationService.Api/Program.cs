using Microsoft.EntityFrameworkCore;
using AuthenticationService.Api.Config;
using AuthenticationService.Api.Data;
using AuthenticationService.Api.Services;
using AuthenticationService.Api.Services.Interfaces;

WebApplicationBuilder builder =
    WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddIdentityConfiguration(
    builder.Configuration);

builder.Services.AddScoped<ITokenService, TokenService>();

WebApplication app = builder.Build();

await using (AsyncServiceScope scope =
    app.Services.CreateAsyncScope())
{
    AuthenticationDbContext context =
        scope.ServiceProvider
            .GetRequiredService<AuthenticationDbContext>();

    if (context.Database.IsRelational())
    {
        await context.Database.MigrateAsync();
    }
    else
    {
        await context.Database.EnsureCreatedAsync();
    }
}

await IdentityDataSeeder.SeedDemoUserAsync(
    app.Services,
    app.Configuration);

app.MapControllers();

app.Run();

namespace AuthenticationService.Api
{
    public sealed class AuthenticationApiAssemblyMarker
    {
    }
}