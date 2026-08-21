using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mediscreen.Tests.Integration.PatientService;

public class TestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(
        options,
        logger,
        encoder)
{
    protected override Task<AuthenticateResult>
        HandleAuthenticateAsync()
    {
        Claim[] claims =
        [
            new Claim(
                ClaimTypes.NameIdentifier,
                "integration-test-user"),

            new Claim(
                ClaimTypes.Name,
                "integration@mediscreen.com"),

            new Claim(
                ClaimTypes.Email,
                "integration@mediscreen.com")
        ];

        ClaimsIdentity identity =
            new(claims, "IntegrationTest");

        ClaimsPrincipal principal = new(identity);

        AuthenticationTicket ticket =
            new(principal, "IntegrationTest");

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}