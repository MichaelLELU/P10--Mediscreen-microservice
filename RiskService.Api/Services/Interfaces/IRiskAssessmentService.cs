using RiskService.Api.Models;

namespace RiskService.Api.Services.Interfaces;

public interface IRiskAssessmentService
{
    Task<RiskAssessment?> AssessAsync(
        int patientId,
        CancellationToken cancellationToken = default);
}