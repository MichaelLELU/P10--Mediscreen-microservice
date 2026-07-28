using Mediscreen.Frontend.Models;
using Mediscreen.Frontend.Services.Interfaces;
using System.Net;
using System.Net.Http.Json;

namespace Mediscreen.Frontend.Services;

public class PatientService(
    IHttpClientFactory httpClientFactory) : IPatientService
{
    private readonly HttpClient _httpClient =
        httpClientFactory.CreateClient("Gateway");

    public async Task<IEnumerable<PatientViewModel>> GetAllAsync()
    {
        return await _httpClient
            .GetFromJsonAsync<IEnumerable<PatientViewModel>>(
                "/gateway/patients")
            ?? [];
    }

    public async Task<PatientViewModel?> GetByIdAsync(int id)
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync($"/gateway/patients/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<PatientViewModel>();
    }

    public async Task<PatientViewModel?> CreateAsync(
        PatientViewModel patient)
    {
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "/gateway/patients",
                patient);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<PatientViewModel>();
    }

    public async Task<bool> UpdateAsync(PatientViewModel patient)
    {
        HttpResponseMessage response =
            await _httpClient.PutAsJsonAsync(
                $"/gateway/patients/{patient.Id}",
                patient);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }
}