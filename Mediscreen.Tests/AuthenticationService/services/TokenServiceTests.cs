using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthenticationService.Api.Models;
using AuthenticationService.Api.Services;
using Microsoft.Extensions.Configuration;

namespace Mediscreen.Tests.AuthenticationService.Services;

public class TokenServiceTests
{
    private static TokenService CreateTokenService()
    {
        Dictionary<string, string?> settings = new()
        {
            ["Jwt:Key"] =
                "UneCleJWTDeTestSuffisammentLongueEtSecurisee123456789",
            ["Jwt:Issuer"] = "Mediscreen.Tests",
            ["Jwt:Audience"] = "Mediscreen.Tests.Client",
            ["Jwt:ExpirationMinutes"] = "60"
        };

        IConfiguration configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

        return new TokenService(configuration);
    }

    private static ApplicationUser CreateUser()
    {
        return new ApplicationUser
        {
            Id = "user-test-123",
            Email = "demo@mediscreen.com",
            UserName = "demo@mediscreen.com"
        };
    }

    [Fact]
    public void CreateToken_ShouldReturnTokenWithUserInformation()
    {
        // Arrange
        TokenService tokenService = CreateTokenService();
        ApplicationUser user = CreateUser();

        // Act
        LoginResponse response =
            tokenService.CreateToken(user);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(response.Token));
        Assert.Equal(user.Email, response.Email);
        Assert.True(response.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public void CreateToken_ShouldContainExpectedClaims()
    {
        // Arrange
        TokenService tokenService = CreateTokenService();
        ApplicationUser user = CreateUser();

        // Act
        LoginResponse response =
            tokenService.CreateToken(user);

        JwtSecurityToken token =
            new JwtSecurityTokenHandler()
                .ReadJwtToken(response.Token);

        // Assert
        Assert.Contains(
            token.Claims,
            claim =>
                claim.Type == JwtRegisteredClaimNames.Sub
                && claim.Value == user.Id);

        Assert.Contains(
            token.Claims,
            claim =>
                claim.Type == JwtRegisteredClaimNames.Email
                && claim.Value == user.Email);

        Assert.Contains(
            token.Claims,
            claim =>
                claim.Type == ClaimTypes.Name
                && claim.Value == user.UserName);

        Assert.Contains(
            token.Claims,
            claim =>
                claim.Type == JwtRegisteredClaimNames.Jti
                && !string.IsNullOrWhiteSpace(claim.Value));
    }

    [Fact]
    public void CreateToken_ShouldUseConfiguredIssuerAndAudience()
    {
        // Arrange
        TokenService tokenService = CreateTokenService();

        // Act
        LoginResponse response =
            tokenService.CreateToken(CreateUser());

        JwtSecurityToken token =
            new JwtSecurityTokenHandler()
                .ReadJwtToken(response.Token);

        // Assert
        Assert.Equal("Mediscreen.Tests", token.Issuer);
        Assert.Contains(
            "Mediscreen.Tests.Client",
            token.Audiences);
    }

    [Fact]
    public void CreateToken_ShouldExpireAfterConfiguredDuration()
    {
        // Arrange
        TokenService tokenService = CreateTokenService();
        DateTime expectedExpiration =
            DateTime.UtcNow.AddMinutes(60);

        // Act
        LoginResponse response =
            tokenService.CreateToken(CreateUser());

        // Assert
        TimeSpan difference =
            response.ExpiresAt - expectedExpiration;

        Assert.InRange(
            Math.Abs(difference.TotalSeconds),
            0,
            5);
    }
}