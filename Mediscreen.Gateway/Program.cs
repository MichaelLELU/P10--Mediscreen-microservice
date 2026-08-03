using Mediscreen.Gateway.Config;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

WebApplicationBuilder builder =
    WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    "ocelot.json",
    optional: false,
    reloadOnChange: true);

builder.Services.AddJwtAuthentication(
    builder.Configuration);

builder.Services.AddOcelot(
    builder.Configuration);

WebApplication app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();