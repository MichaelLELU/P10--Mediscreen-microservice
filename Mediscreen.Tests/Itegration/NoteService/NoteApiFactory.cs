using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using NoteService.Api;
using NoteService.Api.Data;
using Testcontainers.MongoDb;

namespace Mediscreen.Tests.Integration.NoteService;

public class NoteApiFactory :
    WebApplicationFactory<NoteApiAssemblyMarker>,
    IAsyncLifetime
{
    private const string TestJwtKey =
        "Mediscreen-Note-Integration-Test-Key-2026-Long-Enough!";

    private const string TestJwtIssuer =
        "Mediscreen.Gateway";

    private const string TestJwtAudience =
        "Mediscreen.Services";

    private readonly string _databaseName =
        $"NoteIntegrationTests-{Guid.NewGuid()}";

    private readonly MongoDbContainer _mongoContainer =
        new MongoDbBuilder()
            .WithImage("mongo:8.0")
            .Build();

    public NoteApiFactory()
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
                    ["MongoDb:ConnectionString"] =
                        _mongoContainer.GetConnectionString(),

                    ["MongoDb:DatabaseName"] =
                        _databaseName,

                    ["Jwt:Key"] = TestJwtKey,
                    ["Jwt:Issuer"] = TestJwtIssuer,
                    ["Jwt:Audience"] = TestJwtAudience
                };

                configuration.AddInMemoryCollection(settings);
            });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<NoteDbContext>();

            services.RemoveAll<
                DbContextOptions<NoteDbContext>>();

            services.RemoveAll<
                IDbContextOptionsConfiguration<
                    NoteDbContext>>();

            services.RemoveAll<IMongoClient>();

            services.AddSingleton<IMongoClient>(
                _ => new MongoClient(
                    _mongoContainer
                        .GetConnectionString()));

            services.AddDbContext<NoteDbContext>(
                (serviceProvider, options) =>
                {
                    IMongoClient mongoClient =
                        serviceProvider
                            .GetRequiredService<IMongoClient>();

                    options.UseMongoDB(
                        mongoClient,
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
                    NoteTestAuthenticationHandler>(
                        "IntegrationTest",
                        _ => { });
        });
    }

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        MongoClient client = new(
            _mongoContainer.GetConnectionString());

        await client.DropDatabaseAsync(
            _databaseName);
    }

    public new async Task DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();

        Environment.SetEnvironmentVariable(
            "Jwt__Key",
            null);

        Environment.SetEnvironmentVariable(
            "Jwt__Issuer",
            null);

        Environment.SetEnvironmentVariable(
            "Jwt__Audience",
            null);

        await base.DisposeAsync();
    }
}