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

await IdentityDataSeeder.SeedDemoUserAsync(
    app.Services,
    app.Configuration);

app.MapControllers();

app.Run();