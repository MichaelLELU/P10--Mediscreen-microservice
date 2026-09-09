using System.Net.Http.Json;
using RiskService.Api.Clients.Interfaces;
using RiskService.Api.Models;

namespace RiskService.Api.Clients;

public class NoteApiClient(HttpClient httpClient)
    : INoteApiClient
{
    public async Task<IReadOnlyCollection<PatientNoteDto>>
        GetByPatientIdAsync(
            int patientId,
            CancellationToken cancellationToken = default)
    {
        List<PatientNoteDto>? notes =
            await httpClient.GetFromJsonAsync<List<PatientNoteDto>>(
                $"/api/Notes/patient/{patientId}",
                cancellationToken);

        return notes ?? [];
    }
}