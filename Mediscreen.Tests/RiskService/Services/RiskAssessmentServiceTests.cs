using Moq;
using RiskService.Api.Clients.Interfaces;
using RiskService.Api.Models;
using RiskService.Api.Services;

namespace Mediscreen.Tests.RiskEvaluation.Services;

public class RiskAssessmentServiceTests
{
    private readonly Mock<IPatientApiClient> _patientClientMock =
        new();

    private readonly Mock<INoteApiClient> _noteClientMock =
        new();

    private readonly RiskAssessmentService _service;

    public RiskAssessmentServiceTests()
    {
        _service = new RiskAssessmentService(
            _patientClientMock.Object,
            _noteClientMock.Object);
    }

    [Fact]
    public async Task AssessAsync_WhenPatientDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _patientClientMock
            .Setup(client =>
                client.GetByIdAsync(
                    999,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((PatientDto?)null);

        // Act
        RiskAssessment? result =
            await _service.AssessAsync(999);

        // Assert
        Assert.Null(result);

        _noteClientMock.Verify(
            client =>
                client.GetByPatientIdAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task AssessAsync_WithOneTrigger_ShouldReturnNone()
    {
        // Arrange
        ConfigurePatient(
            age: 40,
            gender: "F");

        ConfigureNotes(
            "Poids égal au poids recommandé.");

        // Act
        RiskAssessment? result =
            await _service.AssessAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TriggerCount);
        Assert.Equal(RiskLevel.None, result.RiskLevel);
    }

    [Fact]
    public async Task AssessAsync_ForPatientOverThirtyWithTwoTriggers_ShouldReturnBorderline()
    {
        // Arrange
        ConfigurePatient(
            age: 40,
            gender: "M");

        ConfigureNotes(
            "L'audition est anormale.",
            "Réaction aux médicaments et audition anormale.");

        // Act
        RiskAssessment? result =
            await _service.AssessAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TriggerCount);
        Assert.Equal(
            RiskLevel.Borderline,
            result.RiskLevel);
    }

    [Fact]
    public async Task AssessAsync_ForMaleUnderThirtyWithThreeTriggers_ShouldReturnInDanger()
    {
        // Arrange
        ConfigurePatient(
            age: 20,
            gender: "M");

        ConfigureNotes(
            "Le patient est fumeur.",
            "Respiration anormale et cholestérol élevé.");

        // Act
        RiskAssessment? result =
            await _service.AssessAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TriggerCount);
        Assert.Equal(
            RiskLevel.InDanger,
            result.RiskLevel);
    }

    [Fact]
    public async Task AssessAsync_ForFemaleUnderThirtyWithEightTriggers_ShouldReturnEarlyOnset()
    {
        // Arrange
        ConfigurePatient(
            age: 20,
            gender: "F");

        ConfigureNotes(
            "Anticorps élevés et réaction aux médicaments.",
            "Le patient a commencé à fumer.",
            "Hémoglobine A1C supérieure au niveau recommandé.",
            "Taille, poids, cholestérol et vertige.");

        // Act
        RiskAssessment? result =
            await _service.AssessAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(8, result.TriggerCount);
        Assert.Equal(
            RiskLevel.EarlyOnset,
            result.RiskLevel);
    }

    [Fact]
    public async Task AssessAsync_ShouldIgnoreTriggerCase()
    {
        // Arrange
        ConfigurePatient(
            age: 40,
            gender: "F");

        ConfigureNotes(
            "Le patient présente un POIDS élevé.",
            "Son taux de CHOLESTÉROL est élevé.");

        // Act
        RiskAssessment? result =
            await _service.AssessAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TriggerCount);
        Assert.Equal(
            RiskLevel.Borderline,
            result.RiskLevel);
    }

    [Fact]
    public async Task AssessAsync_ShouldCountRepeatedTriggerOnlyOnce()
    {
        // Arrange
        ConfigurePatient(
            age: 40,
            gender: "M");

        ConfigureNotes(
            "Le patient fume.",
            "Le patient est fumeur.",
            "Le patient souhaite arrêter de fumer.");

        // Act
        RiskAssessment? result =
            await _service.AssessAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TriggerCount);
        Assert.Equal(
            RiskLevel.None,
            result.RiskLevel);
    }

    [Fact]
    public async Task AssessAsync_ShouldCalculatePatientAge()
    {
        // Arrange
        ConfigurePatient(
            age: 25,
            gender: "M");

        ConfigureNotes();

        // Act
        RiskAssessment? result =
            await _service.AssessAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(25, result.Age);
    }

    private void ConfigurePatient(
        int age,
        string gender)
    {
        DateOnly today =
            DateOnly.FromDateTime(DateTime.Today);

        PatientDto patient = new()
        {
            Id = 1,
            FirstName = "Test",
            LastName = "Patient",
            DateOfBirth = today.AddYears(-age),
            Gender = gender
        };

        _patientClientMock
            .Setup(client =>
                client.GetByIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(patient);
    }

    private void ConfigureNotes(
        params string[] contents)
    {
        IReadOnlyCollection<PatientNoteDto> notes =
            contents
                .Select((content, index) =>
                    new PatientNoteDto
                    {
                        Id = $"note-{index + 1}",
                        PatientId = 1,
                        Content = content,
                        CreatedAt = DateTime.UtcNow
                    })
                .ToList();

        _noteClientMock
            .Setup(client =>
                client.GetByPatientIdAsync(
                    1,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(notes);
    }

    [Fact]
    public async Task AssessAsync_ForPatientOverThirtyWithSixTriggers_ShouldReturnInDanger()
    {
        // Arrange
        ConfigurePatient(
            age: 40,
            gender: "M");

        ConfigureNotes(
            "Poids, taille, fumeur, anormal, cholestérol et vertige.");

        // Act
        RiskAssessment? result =
            await _service.AssessAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, result.TriggerCount);
        Assert.Equal(
            RiskLevel.InDanger,
            result.RiskLevel);
    }

    [Fact]
    public async Task AssessAsync_ForPatientOverThirtyWithEightTriggers_ShouldReturnEarlyOnset()
    {
        // Arrange
        ConfigurePatient(
            age: 40,
            gender: "F");

        ConfigureNotes(
            "Poids, taille, fumeur, anormal, cholestérol, vertige, réaction et anticorps.");

        // Act
        RiskAssessment? result =
            await _service.AssessAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(8, result.TriggerCount);
        Assert.Equal(
            RiskLevel.EarlyOnset,
            result.RiskLevel);
    }

    [Fact]
    public async Task AssessAsync_ForMaleUnderThirtyWithFiveTriggers_ShouldReturnEarlyOnset()
    {
        // Arrange
        ConfigurePatient(
            age: 20,
            gender: "M");

        ConfigureNotes(
            "Poids, taille, fumeur, anormal et cholestérol.");

        // Act
        RiskAssessment? result =
            await _service.AssessAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.TriggerCount);
        Assert.Equal(
            RiskLevel.EarlyOnset,
            result.RiskLevel);
    }

    [Fact]
    public async Task AssessAsync_ForFemaleUnderThirtyWithFourTriggers_ShouldReturnInDanger()
    {
        // Arrange
        ConfigurePatient(
            age: 20,
            gender: "F");

        ConfigureNotes(
            "Poids, taille, anormal et cholestérol.");

        // Act
        RiskAssessment? result =
            await _service.AssessAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.TriggerCount);
        Assert.Equal(
            RiskLevel.InDanger,
            result.RiskLevel);
    }

    [Fact]
    public async Task AssessAsync_ShouldIgnoreAccents()
    {
        // Arrange
        ConfigurePatient(
            age: 40,
            gender: "F");

        ConfigureNotes(
            "Hemoglobine A1C élevée.",
            "Le cholesterol est élevé.",
            "Reaction aux médicaments.");

        // Act
        RiskAssessment? result =
            await _service.AssessAsync(1);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(
            3,
            result.TriggerCount);

        Assert.Contains(
            "Hémoglobine A1C",
            result.Triggers);

        Assert.Contains(
            "Cholestérol",
            result.Triggers);

        Assert.Contains(
            "Réaction",
            result.Triggers);
    }
}