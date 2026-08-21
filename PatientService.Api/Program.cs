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

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
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