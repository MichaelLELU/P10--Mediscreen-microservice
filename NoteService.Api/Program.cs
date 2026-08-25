using NoteService.Api.Configurations;
using NoteService.Api.Data;
using NoteService.Api.Repositories;
using NoteService.Api.Repositories.Interfaces;
using Scalar.AspNetCore;

WebApplicationBuilder builder =
    WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddMongoDbConfiguration(
    builder.Configuration);

builder.Services.AddJwtAuthentication(
    builder.Configuration);

builder.Services.AddScoped<
    IPatientNoteRepository,
    PatientNoteRepository>();

WebApplication app = builder.Build();

bool seedRequested =
    args.Length == 2
    && args[0].Equals(
        "seed",
        StringComparison.OrdinalIgnoreCase)
    && args[1].Equals(
        "notes",
        StringComparison.OrdinalIgnoreCase);

if (seedRequested)
{
    if (!app.Environment.IsDevelopment())
    {
        Console.Error.WriteLine(
            "L'import des données de test est interdit en production.");

        Environment.ExitCode = 1;
        return;
    }

    await NoteDataSeeder.SeedAsync(app.Services);

    Console.WriteLine(
        "Les notes de test ont été importées.");

    return;
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi()
        .AllowAnonymous();

    app.MapScalarApiReference()
        .AllowAnonymous();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


namespace NoteService.Api
{
    public sealed class NoteApiAssemblyMarker
    {
    }
}