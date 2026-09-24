using System.Net;
using System.Net.Http.Json;
using PatientService.Api.Models;

namespace Mediscreen.Tests.Integration.PatientService;

public class PatientsApiTests :
    IClassFixture<PatientApiFactory>,
    IAsyncLifetime
{
    private readonly PatientApiFactory _factory;
    private readonly HttpClient _client;

    public PatientsApiTests(PatientApiFactory factory)
    {
        _factory = factory;

        _client = factory.CreateClient(
            new()
            {
                BaseAddress = new Uri("https://localhost")
            });
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }

    private static Patient CreatePatient(
        int id = 0,
        string firstName = "John",
        string lastName = "Doe")
    {
        return new Patient
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = new DateOnly(1980, 1, 1),
            Gender = "M",
            Address = "1 rue de Paris",
            PhoneNumber = "0102030405"
        };
    }

    [Fact]
    public async Task GetAll_WhenDatabaseIsEmpty_ShouldReturnEmptyList()
    {
        // Act
        HttpResponseMessage response =
            await _client.GetAsync("/api/Patients");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        List<Patient>? patients =
            await response.Content
                .ReadFromJsonAsync<List<Patient>>();

        Assert.NotNull(patients);
        Assert.Empty(patients);
    }

    [Fact]
    public async Task Create_ShouldSavePatientAndReturnCreated()
    {
        // Arrange
        Patient patient = CreatePatient();

        // Act
        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/Patients",
                patient);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        Patient? createdPatient =
            await response.Content
                .ReadFromJsonAsync<Patient>();

        Assert.NotNull(createdPatient);
        Assert.True(createdPatient.Id > 0);
        Assert.Equal("John", createdPatient.FirstName);
        Assert.Equal("Doe", createdPatient.LastName);
    }

    [Fact]
    public async Task Create_ThenGetById_ShouldReturnSavedPatient()
    {
        // Arrange
        HttpResponseMessage createResponse =
            await _client.PostAsJsonAsync(
                "/api/Patients",
                CreatePatient());

        Patient? createdPatient =
            await createResponse.Content
                .ReadFromJsonAsync<Patient>();

        Assert.NotNull(createdPatient);

        // Act
        HttpResponseMessage response =
            await _client.GetAsync(
                $"/api/Patients/{createdPatient.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Patient? returnedPatient =
            await response.Content
                .ReadFromJsonAsync<Patient>();

        Assert.NotNull(returnedPatient);
        Assert.Equal(createdPatient.Id, returnedPatient.Id);
        Assert.Equal("John", returnedPatient.FirstName);
    }

    [Fact]
    public async Task GetById_WhenPatientDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        HttpResponseMessage response =
            await _client.GetAsync("/api/Patients/999");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenPatientExists_ShouldReturnNoContent()
    {
        // Arrange
        HttpResponseMessage createResponse =
            await _client.PostAsJsonAsync(
                "/api/Patients",
                CreatePatient());

        Patient? patient =
            await createResponse.Content
                .ReadFromJsonAsync<Patient>();

        Assert.NotNull(patient);

        patient.FirstName = "Michael";
        patient.LastName = "Martin";
        patient.Address = "10 avenue de Lyon";

        // Act
        HttpResponseMessage updateResponse =
            await _client.PutAsJsonAsync(
                $"/api/Patients/{patient.Id}",
                patient);

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            updateResponse.StatusCode);

        Patient? updatedPatient =
            await _client.GetFromJsonAsync<Patient>(
                $"/api/Patients/{patient.Id}");

        Assert.NotNull(updatedPatient);
        Assert.Equal("Michael", updatedPatient.FirstName);
        Assert.Equal("Martin", updatedPatient.LastName);
        Assert.Equal(
            "10 avenue de Lyon",
            updatedPatient.Address);
    }

    [Fact]
    public async Task Create_WithInvalidPatient_ShouldReturnBadRequest()
    {
        // Arrange
        Patient invalidPatient = CreatePatient();
        invalidPatient.FirstName = string.Empty;
        invalidPatient.LastName = string.Empty;

        // Act
        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/Patients",
                invalidPatient);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}