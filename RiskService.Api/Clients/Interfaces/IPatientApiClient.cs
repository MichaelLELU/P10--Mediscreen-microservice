using RiskService.Api.Models;

namespace RiskService.Api.Clients.Interfaces;

public interface IPatientApiClient
{
    Task<PatientDto?> GetByIdAsync(
        int patientId,
        CancellationToken cancellationToken = default);
}
