using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RiskService.Api.Controllers;
using RiskService.Api.Models;
using RiskService.Api.Services.Interfaces;

namespace Mediscreen.Tests.RiskEvaluation.Controllers;

public class RiskControllerTests
{
    private readonly Mock<IRiskAssessmentService> _serviceMock =
        new();

    private readonly RiskController _controller;

    public RiskControllerTests()
    {
        _controller =
            new RiskController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAssessment_WhenPatientExists_ShouldReturnOk()
    {
        // Arrange
        RiskAssessment assessment = new()
        {
            PatientId = 3,
            PatientName = "Test TestInDanger",
            Age = 22,
            TriggerCount = 3,
            RiskLevel = RiskLevel.InDanger
        };

        _serviceMock
            .Setup(service =>
                service.AssessAsync(
                    3,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);

        // Act
        ActionResult<RiskAssessment> result =
            await _controller.GetAssessment(
                3,
                CancellationToken.None);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(
                result.Result);

        RiskAssessment returnedAssessment =
            Assert.IsType<RiskAssessment>(
                okResult.Value);

        Assert.Equal(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        Assert.Equal(
            RiskLevel.InDanger,
            returnedAssessment.RiskLevel);
    }

    [Fact]
    public async Task GetAssessment_WhenPatientDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        _serviceMock
            .Setup(service =>
                service.AssessAsync(
                    999,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((RiskAssessment?)null);

        // Act
        ActionResult<RiskAssessment> result =
            await _controller.GetAssessment(
                999,
                CancellationToken.None);

        // Assert
        NotFoundObjectResult notFoundResult =
            Assert.IsType<NotFoundObjectResult>(
                result.Result);

        ProblemDetails problem =
            Assert.IsType<ProblemDetails>(
                notFoundResult.Value);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            problem.Status);

        Assert.Equal(
            "Patient introuvable",
            problem.Title);
    }
}