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

    private readonly string _databaseName =
        $"AuthenticationIntegrationTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(
            (_, configuration) =>
            {
                Dictionary<string, string?> settings = new()
                {
                    ["DemoUser:Email"] = DemoEmail,
                    ["DemoUser:Password"] = DemoPassword,

                    ["Jwt:Key"] =
                        "CleJWTIntegrationMediscreenSuffisammentLongue123456789",

                    ["Jwt:Issuer"] =
                        "Mediscreen.IntegrationTests",

                    ["Jwt:Audience"] =
                        "Mediscreen.IntegrationTests.Client",

                    ["Jwt:ExpirationMinutes"] = "60"
                };

                configuration.AddInMemoryCollection(settings);
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
                    options.UseInMemoryDatabase(_databaseName);
                });
        });
    }
}