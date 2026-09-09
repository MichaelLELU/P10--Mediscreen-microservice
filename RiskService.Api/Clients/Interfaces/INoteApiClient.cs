using RiskService.Api.Models;

namespace RiskService.Api.Clients.Interfaces;

public interface INoteApiClient
{
    Task<IReadOnlyCollection<PatientNoteDto>> GetByPatientIdAsync(
        int patientId,
        CancellationToken cancellationToken = default);
}