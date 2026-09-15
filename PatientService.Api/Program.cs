using Microsoft.EntityFrameworkCore;
using PatientService.Api.Configurations;
using PatientService.Api.Data;
using PatientService.Api.Repositories;
using PatientService.Api.Repositories.Interfaces;
using PatientService.Api.Utils;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "PatientDatabase")));

builder.Services.AddScoped<
    IPatientRepository,
    PatientRepository>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddJwtAuthentication(builder.Configuration);

WebApplication app = builder.Build();

await using (AsyncServiceScope scope =
    app.Services.CreateAsyncScope())
{
    PatientDbContext context =
        scope.ServiceProvider
            .GetRequiredService<PatientDbContext>();

    if (context.Database.IsRelational())
    {
        await context.Database.MigrateAsync();
    }
    else
    {
        await context.Database.EnsureCreatedAsync();
    }
}

if (!app.Environment.IsEnvironment("Testing"))
{
    await PatientDataSeeder.SeedAsync(
        app.Services);
}

app.UseExceptionHandler();

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

namespace PatientService.Api
{
    public sealed class PatientApiAssemblyMarker
    {
    }
}