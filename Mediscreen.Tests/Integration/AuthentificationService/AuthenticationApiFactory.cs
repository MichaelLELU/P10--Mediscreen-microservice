using AuthenticationService.Api;
using AuthenticationService.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mediscreen.Tests.Integration.AuthenticationService;

public class AuthenticationApiFactory :
    WebApplicationFactory<AuthenticationApiAssemblyMarker>
{
    public const string DemoEmail =
        "integration@mediscreen.com";

    public const string DemoPassword =
        "Integration123!";

    private const string TestConnectionString =
        "Server=localhost;" +
        "Database=AuthenticationIntegrationTests;" +
        "User Id=sa;" +
        "Password=Integration123!;" +
        "TrustServerCertificate=True";

    private const string TestJwtKey =
        "CleJWTIntegrationMediscreenSuffisammentLongue123456789";

    private const string TestJwtIssuer =
        "Mediscreen.IntegrationTests";

    private const string TestJwtAudience =
        "Mediscreen.IntegrationTests.Client";

    private readonly string _databaseName =
        $"AuthenticationIntegrationTests-{Guid.NewGuid()}";

    public AuthenticationApiFactory()
    {
        Environment.SetEnvironmentVariable(
            "ConnectionStrings__AuthenticationDatabase",
            TestConnectionString);

        Environment.SetEnvironmentVariable(
            "DemoUser__Email",
            DemoEmail);

        Environment.SetEnvironmentVariable(
            "DemoUser__Password",
            DemoPassword);

        Environment.SetEnvironmentVariable(
            "Jwt__Key",
            TestJwtKey);

        Environment.SetEnvironmentVariable(
            "Jwt__Issuer",
            TestJwtIssuer);

        Environment.SetEnvironmentVariable(
            "Jwt__Audience",
            TestJwtAudience);

        Environment.SetEnvironmentVariable(
            "Jwt__ExpirationMinutes",
            "60");
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(
            (_, configuration) =>
            {
                Dictionary<string, string?> settings = new()
                {
                    ["ConnectionStrings:AuthenticationDatabase"] =
                        TestConnectionString,

                    ["DemoUser:Email"] =
                        DemoEmail,

                    ["DemoUser:Password"] =
                        DemoPassword,

                    ["Jwt:Key"] =
                        TestJwtKey,

                    ["Jwt:Issuer"] =
                        TestJwtIssuer,

                    ["Jwt:Audience"] =
                        TestJwtAudience,

                    ["Jwt:ExpirationMinutes"] =
                        "60"
                };

                configuration.AddInMemoryCollection(
                    settings);
            });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<AuthenticationDbContext>();

            services.RemoveAll<
                DbContextOptions<AuthenticationDbContext>>();

            services.RemoveAll<
                IDbContextOptionsConfiguration<
                    AuthenticationDbContext>>();

            services.AddDbContext<AuthenticationDbContext>(
                options =>
                {
                    options.UseInMemoryDatabase(
                        _databaseName);
                });
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Environment.SetEnvironmentVariable(
                "ConnectionStrings__AuthenticationDatabase",
                null);

            Environment.SetEnvironmentVariable(
                "DemoUser__Email",
                null);

            Environment.SetEnvironmentVariable(
                "DemoUser__Password",
                null);

            Environment.SetEnvironmentVariable(
                "Jwt__Key",
                null);

            Environment.SetEnvironmentVariable(
                "Jwt__Issuer",
                null);

            Environment.SetEnvironmentVariable(
                "Jwt__Audience",
                null);

            Environment.SetEnvironmentVariable(
                "Jwt__ExpirationMinutes",
                null);
        }

        base.Dispose(disposing);
    }
}