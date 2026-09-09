using System.Text.Json.Serialization;
using RiskService.Api.Clients;
using RiskService.Api.Clients.Interfaces;
using RiskService.Api.Configurations;
using RiskService.Api.Services;
using RiskService.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Controllers et sérialisation JSON
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

// OpenAPI
builder.Services.AddOpenApi();

// Accès à la requête HTTP entrante
builder.Services.AddHttpContextAccessor();

// Transmission du JWT aux autres microservices
builder.Services.AddTransient<JwtForwardingHandler>();

// Client HTTP du PatientService
builder.Services.AddHttpClient<
    IPatientApiClient,
    PatientApiClient>(httpClient =>
    {
        string patientServiceUrl =
            builder.Configuration[
                "ServiceUrls:PatientService"]
            ?? throw new InvalidOperationException(
                "L'adresse du PatientService est absente.");

        httpClient.BaseAddress =
            new Uri(patientServiceUrl);
    })
.AddHttpMessageHandler<JwtForwardingHandler>();

// Client HTTP du NoteService
builder.Services.AddHttpClient<
    INoteApiClient,
    NoteApiClient>(httpClient =>
    {
        string noteServiceUrl =
            builder.Configuration[
                "ServiceUrls:NoteService"]
            ?? throw new InvalidOperationException(
                "L'adresse du NoteService est absente.");

        httpClient.BaseAddress =
            new Uri(noteServiceUrl);
    })
.AddHttpMessageHandler<JwtForwardingHandler>();

// Service d'évaluation du risque
builder.Services.AddScoped<
    IRiskAssessmentService,
    RiskAssessmentService>();

builder.Services.AddJwtAuthentication(
    builder.Configuration);

// Cette instruction doit être placée après tous les Add...
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();