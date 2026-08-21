using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using NoteService.Api.Data;

namespace NoteService.Api.Configurations;

public static class MongoDbConfiguration
{
    public static IServiceCollection AddMongoDbConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString =
            configuration["MongoDb:ConnectionString"]
            ?? throw new InvalidOperationException(
                "La chaîne de connexion MongoDB est manquante.");

        string databaseName =
            configuration["MongoDb:DatabaseName"]
            ?? throw new InvalidOperationException(
                "Le nom de la base MongoDB est manquant.");

        services.AddSingleton<IMongoClient>(
            new MongoClient(connectionString));

        services.AddDbContext<NoteDbContext>(
            (serviceProvider, options) =>
            {
                IMongoClient mongoClient =
                    serviceProvider
                        .GetRequiredService<IMongoClient>();

                options.UseMongoDB(
                    mongoClient,
                    databaseName);
            });

        return services;
    }
}