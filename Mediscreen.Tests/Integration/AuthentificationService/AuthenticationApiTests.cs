using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using AuthenticationService.Api.Models;

namespace Mediscreen.Tests.Integration.AuthenticationService;

public class AuthenticationApiTests :
    IClassFixture<AuthenticationApiFactory>
{
    private readonly HttpClient _client;

    public AuthenticationApiTests(
        AuthenticationApiFactory factory)
    {
        _client = factory.CreateClient(
            new()
            {
                BaseAddress = new Uri("https://localhost")
            });
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnJwt()
    {
        // Arrange
        LoginRequest request = new()
        {
            Email = AuthenticationApiFactory.DemoEmail,
            Password = AuthenticationApiFactory.DemoPassword
        };

        // Act
        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        LoginResponse? loginResponse =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(loginResponse);
        Assert.False(
            string.IsNullOrWhiteSpace(loginResponse.Token));

        Assert.Equal(
            AuthenticationApiFactory.DemoEmail,
            loginResponse.Email);

        Assert.True(
            loginResponse.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnValidJwtClaims()
    {
        // Arrange
        LoginRequest request = new()
        {
            Email = AuthenticationApiFactory.DemoEmail,
            Password = AuthenticationApiFactory.DemoPassword
        };

        // Act
        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                request);

        LoginResponse? loginResponse =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(loginResponse);

        JwtSecurityToken token =
            new JwtSecurityTokenHandler()
                .ReadJwtToken(loginResponse.Token);

        // Assert
        Assert.Equal(
            "Mediscreen.IntegrationTests",
            token.Issuer);

        Assert.Contains(
            "Mediscreen.IntegrationTests.Client",
            token.Audiences);

        Assert.Contains(
            token.Claims,
            claim =>
                claim.Type == JwtRegisteredClaimNames.Email
                && claim.Value ==
                    AuthenticationApiFactory.DemoEmail);

        Assert.True(token.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        // Arrange
        LoginRequest request = new()
        {
            Email = AuthenticationApiFactory.DemoEmail,
            Password = "MauvaisMotDePasse123!"
        };

        // Act
        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_ShouldReturnUnauthorized()
    {
        // Arrange
        LoginRequest request = new()
        {
            Email = "inconnu@mediscreen.com",
            Password = AuthenticationApiFactory.DemoPassword
        };

        // Act
        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Login_WithInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        LoginRequest request = new()
        {
            Email = "adresse-invalide",
            Password = string.Empty
        };

        // Act
        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                request);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}