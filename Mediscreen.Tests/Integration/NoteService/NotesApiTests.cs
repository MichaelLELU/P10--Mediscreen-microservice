using Microsoft.AspNetCore.Mvc.Testing;
using NoteService.Api.Models;
using System.Net;
using System.Net.Http.Json;

namespace Mediscreen.Tests.Integration.NoteService;

public class NotesApiTests :
    IClassFixture<NoteApiFactory>,
    IAsyncLifetime
{
    private readonly NoteApiFactory _factory;
    private readonly HttpClient _client;

    public NotesApiTests(NoteApiFactory factory)
    {
        _factory = factory;

        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
    }

    public Task InitializeAsync()
    {
        return _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Create_ShouldSaveNoteAndReturnCreated()
    {
        // Arrange
        CreatePatientNoteRequest request = new()
        {
            PatientId = 1,
            Content = "Première consultation.\nFatigue persistante."
        };

        // Act
        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/Notes",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        PatientNote? createdNote =
            await response.Content
                .ReadFromJsonAsync<PatientNote>();

        Assert.NotNull(createdNote);
        Assert.False(
            string.IsNullOrWhiteSpace(createdNote.Id));

        Assert.Equal(
            request.PatientId,
            createdNote.PatientId);

        Assert.Equal(
            request.Content,
            createdNote.Content);
    }

    [Fact]
    public async Task GetByPatientId_ShouldReturnSavedNotes()
    {
        // Arrange
        const int patientId = 987654;

        await CreateNoteAsync(
            patientId,
            "Première note d'intégration.");

        await CreateNoteAsync(
            patientId,
            "Deuxième note d'intégration.");

        await CreateNoteAsync(
            patientId + 1,
            "Note appartenant à un autre patient.");

        // Act
        HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/Notes/patient/{patientId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        List<PatientNote>? notes =
            await response.Content
                .ReadFromJsonAsync<List<PatientNote>>();

        Assert.NotNull(notes);
        Assert.Equal(2, notes.Count);

        Assert.All(
            notes,
            note => Assert.Equal(
                patientId,
                note.PatientId));
    }


    [Fact]
    public async Task GetById_WhenNoteExists_ShouldReturnNote()
    {
        // Arrange
        PatientNote createdNote =
            await CreateNoteAsync(
                patientId: 1,
                content: "Note à récupérer.");

        // Act
        HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/Notes/{createdNote.Id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        PatientNote? returnedNote =
            await response.Content
                .ReadFromJsonAsync<PatientNote>();

        Assert.NotNull(returnedNote);
        Assert.Equal(
            createdNote.Id,
            returnedNote.Id);
    }

    [Fact]
    public async Task GetById_WhenNoteDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        HttpResponseMessage response =
            await _client.GetAsync(
                "/api/Notes/507f1f77bcf86cd799439099");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenNoteExists_ShouldModifyContent()
    {
        // Arrange
        PatientNote createdNote =
            await CreateNoteAsync(
                patientId: 1,
                content: "Ancien contenu.");

        UpdatePatientNoteRequest request = new()
        {
            Content = "Nouveau contenu.\nDeuxième ligne."
        };

        // Act
        HttpResponseMessage response =
            await _client.PutAsJsonAsync(
                $"/api/Notes/{createdNote.Id}",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        PatientNote? updatedNote =
            await response.Content
                .ReadFromJsonAsync<PatientNote>();

        Assert.NotNull(updatedNote);
        Assert.Equal(
            request.Content,
            updatedNote.Content);

        Assert.NotNull(updatedNote.UpdatedAt);
    }

    [Fact]
    public async Task Delete_WhenNoteExists_ShouldRemoveNote()
    {
        // Arrange
        PatientNote createdNote =
            await CreateNoteAsync(
                patientId: 1,
                content: "Note à supprimer.");

        // Act
        HttpResponseMessage deleteResponse =
            await _client.DeleteAsync(
                $"/api/Notes/{createdNote.Id}");

        HttpResponseMessage getResponse =
            await _client.GetAsync(
                $"/api/Notes/{createdNote.Id}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        Assert.Equal(
            HttpStatusCode.NotFound,
            getResponse.StatusCode);
    }

    private async Task<PatientNote> CreateNoteAsync(
        int patientId,
        string content)
    {
        CreatePatientNoteRequest request = new()
        {
            PatientId = patientId,
            Content = content
        };

        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/Notes",
                request);

        response.EnsureSuccessStatusCode();

        PatientNote? note =
            await response.Content
                .ReadFromJsonAsync<PatientNote>();

        return Assert.IsType<PatientNote>(note);
    }
}