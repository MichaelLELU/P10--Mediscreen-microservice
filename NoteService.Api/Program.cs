using Scalar.AspNetCore;
using NoteService.Api.Configurations;
using NoteService.Api.Repositories;
using NoteService.Api.Repositories.Interfaces;

WebApplicationBuilder builder =
    WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddMongoDbConfiguration(
    builder.Configuration);

builder.Services.AddScoped<
    IPatientNoteRepository,
    PatientNoteRepository>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();