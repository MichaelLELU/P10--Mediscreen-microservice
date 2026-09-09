using System.Net;
using System.Net.Http.Json;
using RiskService.Api.Clients.Interfaces;
using RiskService.Api.Models;

namespace RiskService.Api.Clients;

public class PatientApiClient(HttpClient httpClient)
    : IPatientApiClient
{
    public async Task<PatientDto?> GetByIdAsync(
        int patientId,
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response =
            await httpClient.GetAsync(
                $"/api/Patients/{patientId}",
                cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<PatientDto>(
                cancellationToken);
    }
}