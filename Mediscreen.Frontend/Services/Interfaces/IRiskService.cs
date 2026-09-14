using Mediscreen.Frontend.Models;

namespace Mediscreen.Frontend.Services.Interfaces;

public interface IRiskService
{
    Task<RiskAssessmentViewModel?> GetByPatientIdAsync(
        int patientId);
}