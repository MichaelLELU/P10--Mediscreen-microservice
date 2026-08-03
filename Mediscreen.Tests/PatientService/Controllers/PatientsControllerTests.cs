using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PatientService.Api.Controllers;
using PatientService.Api.Models;
using PatientService.Api.Repositories.Interfaces;

namespace Mediscreen.Tests.PatientService.Controllers;

public class PatientsControllerTests
{
    private readonly Mock<IPatientRepository> _repositoryMock = new();
    private readonly PatientsController _controller;

    public PatientsControllerTests()
    {
        _controller = new PatientsController(_repositoryMock.Object);
    }

    private static Patient CreatePatient(int id = 1)
    {
        return new Patient
        {
            Id = id,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateOnly(1980, 1, 1),
            Gender = "M",
            Address = "1 rue de Paris",
            PhoneNumber = "0102030405"
        };
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithPatients()
    {
        // Arrange
        List<Patient> patients =
        [
            CreatePatient(1),
            CreatePatient(2)
        ];

        _repositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(patients);

        // Act
        ActionResult<IEnumerable<Patient>> result =
            await _controller.GetAll();

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        IEnumerable<Patient> returnedPatients =
            Assert.IsAssignableFrom<IEnumerable<Patient>>(
                okResult.Value);

        Assert.Equal(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        Assert.Equal(2, returnedPatients.Count());
    }

    [Fact]
    public async Task GetById_WhenPatientExists_ShouldReturnOk()
    {
        // Arrange
        Patient patient = CreatePatient();

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(patient.Id))
            .ReturnsAsync(patient);

        // Act
        ActionResult<Patient> result =
            await _controller.GetById(patient.Id);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        Patient returnedPatient =
            Assert.IsType<Patient>(okResult.Value);

        Assert.Equal(patient.Id, returnedPatient.Id);
        Assert.Equal("John", returnedPatient.FirstName);
    }

    [Fact]
    public async Task GetById_WhenPatientDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        const int patientId = 999;

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(patientId))
            .ReturnsAsync((Patient?)null);

        // Act
        ActionResult<Patient> result =
            await _controller.GetById(patientId);

        // Assert
        NotFoundObjectResult notFoundResult =
            Assert.IsType<NotFoundObjectResult>(result.Result);

        ProblemDetails problem =
            Assert.IsType<ProblemDetails>(
                notFoundResult.Value);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            problem.Status);

        Assert.Equal("Patient introuvable", problem.Title);
        Assert.Contains(patientId.ToString(), problem.Detail);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedPatient()
    {
        // Arrange
        Patient patient = CreatePatient(0);
        Patient createdPatient = CreatePatient(1);

        _repositoryMock
            .Setup(repository => repository.AddAsync(patient))
            .ReturnsAsync(createdPatient);

        // Act
        ActionResult<Patient> result =
            await _controller.Create(patient);

        // Assert
        CreatedAtActionResult createdResult =
            Assert.IsType<CreatedAtActionResult>(
                result.Result);

        Patient returnedPatient =
            Assert.IsType<Patient>(createdResult.Value);

        Assert.Equal(
            nameof(PatientsController.GetById),
            createdResult.ActionName);

        Assert.Equal(1, createdResult.RouteValues?["id"]);
        Assert.Equal(createdPatient.Id, returnedPatient.Id);
    }

    [Fact]
    public async Task Update_WhenIdsAreDifferent_ShouldReturnBadRequest()
    {
        // Arrange
        Patient patient = CreatePatient(2);

        // Act
        IActionResult result =
            await _controller.Update(1, patient);

        // Assert
        BadRequestObjectResult badRequestResult =
            Assert.IsType<BadRequestObjectResult>(result);

        ProblemDetails problem =
            Assert.IsType<ProblemDetails>(
                badRequestResult.Value);

        Assert.Equal(
            StatusCodes.Status400BadRequest,
            problem.Status);

        Assert.Equal(
            "Identifiants différents",
            problem.Title);

        _repositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Patient>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WhenPatientDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        Patient patient = CreatePatient();

        _repositoryMock
            .Setup(repository => repository.UpdateAsync(patient))
            .ReturnsAsync(false);

        // Act
        IActionResult result =
            await _controller.Update(patient.Id, patient);

        // Assert
        NotFoundObjectResult notFoundResult =
            Assert.IsType<NotFoundObjectResult>(result);

        ProblemDetails problem =
            Assert.IsType<ProblemDetails>(
                notFoundResult.Value);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            problem.Status);

        Assert.Equal("Patient introuvable", problem.Title);
    }

    [Fact]
    public async Task Update_WhenPatientExists_ShouldReturnNoContent()
    {
        // Arrange
        Patient patient = CreatePatient();

        _repositoryMock
            .Setup(repository => repository.UpdateAsync(patient))
            .ReturnsAsync(true);

        // Act
        IActionResult result =
            await _controller.Update(patient.Id, patient);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _repositoryMock.Verify(
            repository => repository.UpdateAsync(patient),
            Times.Once);
    }
}