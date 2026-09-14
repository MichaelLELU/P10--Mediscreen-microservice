using System.Net;
using System.Net.Http.Json;
using Mediscreen.Frontend.Models;
using Mediscreen.Frontend.Services.Interfaces;

namespace Mediscreen.Frontend.Services;

public class RiskService(
    IHttpClientFactory httpClientFactory) : IRiskService
{
    private readonly HttpClient _httpClient =
        httpClientFactory.CreateClient("Gateway");

    public async Task<RiskAssessmentViewModel?>
        GetByPatientIdAsync(int patientId)
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync(
                $"/gateway/risk/{patientId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<RiskAssessmentViewModel>();
    }
}