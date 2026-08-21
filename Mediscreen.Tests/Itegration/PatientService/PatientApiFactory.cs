using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using PatientService.Api.Data;
using PatientService.Api;

namespace Mediscreen.Tests.Integration.PatientService;

public class PatientApiFactory :
    WebApplicationFactory<PatientApiAssemblyMarker>
{
    private readonly string _databaseName =
        $"PatientIntegrationTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<PatientDbContext>();

            services.RemoveAll<
                DbContextOptions<PatientDbContext>>();

            services.RemoveAll<
                IDbContextOptionsConfiguration<PatientDbContext>>();

            services.AddDbContext<PatientDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
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