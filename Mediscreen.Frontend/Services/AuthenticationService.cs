using System.Net;
using System.Net.Http.Json;
using Mediscreen.Frontend.Models;
using Mediscreen.Frontend.Services.Interfaces;

namespace Mediscreen.Frontend.Services;

public class AuthenticationService(
    IHttpClientFactory httpClientFactory)
    : IAuthenticationService
{
    private readonly HttpClient _httpClient =
        httpClientFactory.CreateClient("Gateway");

    public async Task<LoginResponse?> LoginAsync(
        LoginRequest request)
    {
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "/gateway/auth/login",
                request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<LoginResponse>();
    }
}