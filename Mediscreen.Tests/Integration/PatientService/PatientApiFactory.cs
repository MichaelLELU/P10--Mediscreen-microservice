using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PatientService.Api;
using PatientService.Api.Data;

namespace Mediscreen.Tests.Integration.PatientService;

public class PatientApiFactory :
    WebApplicationFactory<PatientApiAssemblyMarker>
{
    private const string TestJwtKey =
        "Mediscreen-Patient-Integration-Test-Key-2026-Long-Enough!";

    private const string TestJwtIssuer =
        "Mediscreen.Gateway";

    private const string TestJwtAudience =
        "Mediscreen.Services";

    private readonly string _databaseName =
        $"PatientIntegrationTests-{Guid.NewGuid()}";

    public PatientApiFactory()
    {
        Environment.SetEnvironmentVariable(
            "Jwt__Key",
            TestJwtKey);

        Environment.SetEnvironmentVariable(
            "Jwt__Issuer",
            TestJwtIssuer);

        Environment.SetEnvironmentVariable(
            "Jwt__Audience",
            TestJwtAudience);
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
                    ["Jwt:Key"] = TestJwtKey,
                    ["Jwt:Issuer"] = TestJwtIssuer,
                    ["Jwt:Audience"] = TestJwtAudience
                };

                configuration.AddInMemoryCollection(settings);
            });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<PatientDbContext>();

            services.RemoveAll<
                DbContextOptions<PatientDbContext>>();

            services.RemoveAll<
                IDbContextOptionsConfiguration<
                    PatientDbContext>>();

            services.AddDbContext<PatientDbContext>(
                options =>
                {
                    options.UseInMemoryDatabase(
                        _databaseName);
                });
        });

        builder.ConfigureTestServices(services =>
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        "IntegrationTest";

                    options.DefaultChallengeScheme =
                        "IntegrationTest";
                })
                .AddScheme<
                    AuthenticationSchemeOptions,
                    TestAuthenticationHandler>(
                        "IntegrationTest",
                        _ => { });
        });
    }

    public async Task ResetDatabaseAsync()
    {
        using IServiceScope scope =
            Services.CreateScope();

        PatientDbContext context =
            scope.ServiceProvider
                .GetRequiredService<PatientDbContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}