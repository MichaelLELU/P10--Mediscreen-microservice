using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using NoteService.Api.Models;

namespace NoteService.Api.Data;

public static class NoteDataSeeder
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider)
    {
        using IServiceScope scope =
            serviceProvider.CreateScope();

        NoteDbContext context =
            scope.ServiceProvider
                .GetRequiredService<NoteDbContext>();

        await context.Database.EnsureCreatedAsync();

        (int PatientId, string Content)[] sourceNotes =
        [
            (
                1,
                "Le patient déclare qu'il 'se sent très bien' Poids égal ou inférieur au poids recommandé"
            ),
            (
                2,
                "Le patient déclare qu'il ressent beaucoup de stress au travail Il se plaint également que son audition est anormale dernièrement"
            ),
            (
                2,
                "Le patient déclare avoir fait une réaction aux médicaments au cours des 3 derniers mois Il remarque également que son audition continue d'être anormale"
            ),
            (
                3,
                "Le patient déclare qu'il fume depuis peu"
            ),
            (
                3,
                "Le patient déclare qu'il est fumeur et qu'il a cessé de fumer l'année dernière Il se plaint également de crises d’apnée respiratoire anormales Tests de laboratoire indiquant un taux de cholestérol LDL élevé"
            ),
            (
                4,
                "Le patient déclare qu'il lui est devenu difficile de monter les escaliers Il se plaint également d’être essoufflé Tests de laboratoire indiquant que les anticorps sont élevés Réaction aux médicaments"
            ),
            (
                4,
                "Le patient déclare qu'il a mal au dos lorsqu'il reste assis pendant longtemps"
            ),
            (
                4,
                " Le patient déclare avoir commencé à fumer depuis peu Hémoglobine A1C supérieure au niveau recommandé"
            ),
            (
                4,
                "Taille, Poids, Cholestérol, Vertige et Réaction"
            )
        ];

        DateTime firstNoteDate =
            DateTime.UtcNow.AddMinutes(-sourceNotes.Length);

        List<PatientNote> missingNotes = [];

        for (int index = 0;
             index < sourceNotes.Length;
             index++)
        {
            (int patientId, string content) =
                sourceNotes[index];

            bool alreadyExists =
                await context.Notes.AnyAsync(
                    note =>
                        note.PatientId == patientId
                        && note.Content == content);

            if (alreadyExists)
            {
                continue;
            }

            missingNotes.Add(new PatientNote
            {
                Id = ObjectId
                    .GenerateNewId()
                    .ToString(),

                PatientId = patientId,
                Content = content,

                CreatedAt =
                    firstNoteDate.AddMinutes(index)
            });
        }

        if (missingNotes.Count == 0)
        {
            return;
        }

        context.Database.AutoTransactionBehavior =
            AutoTransactionBehavior.Never;

        context.Notes.AddRange(missingNotes);

        await context.SaveChangesAsync();
    }
}