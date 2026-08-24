using System.Net;
using System.Net.Http.Json;
using Mediscreen.Frontend.Models;
using Mediscreen.Frontend.Services.Interfaces;

namespace Mediscreen.Frontend.Services;

public class NoteService(
    IHttpClientFactory httpClientFactory) : INoteService
{
    private readonly HttpClient _httpClient =
        httpClientFactory.CreateClient("Gateway");

    public async Task<IReadOnlyList<PatientNoteViewModel>>
        GetByPatientIdAsync(int patientId)
    {
        return await _httpClient
            .GetFromJsonAsync<IReadOnlyList<PatientNoteViewModel>>(
                $"/gateway/notes/patient/{patientId}")
            ?? [];
    }

    public async Task<PatientNoteViewModel?> CreateAsync(
        CreatePatientNoteViewModel request)
    {
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "/gateway/notes",
                request);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<PatientNoteViewModel>();
    }

    public async Task<PatientNoteViewModel?> UpdateAsync(
        string id,
        UpdatePatientNoteViewModel request)
    {
        HttpResponseMessage response =
            await _httpClient.PutAsJsonAsync(
                $"/gateway/notes/{id}",
                new
                {
                    request.Content
                });

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<PatientNoteViewModel>();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        HttpResponseMessage response =
            await _httpClient.DeleteAsync(
                $"/gateway/notes/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }
}