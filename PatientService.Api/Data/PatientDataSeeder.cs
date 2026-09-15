using Microsoft.EntityFrameworkCore;
using PatientService.Api.Models;

namespace PatientService.Api.Data;

public static class PatientDataSeeder
{
    public static async Task SeedAsync(
        IServiceProvider serviceProvider)
    {
        using IServiceScope scope =
            serviceProvider.CreateScope();

        PatientDbContext context =
            scope.ServiceProvider
                .GetRequiredService<PatientDbContext>();

        if (await context.Patients.AnyAsync())
        {
            return;
        }

        Patient[] patients =
        [
            new Patient
            {
                FirstName = "Test",
                LastName = "TestNone",
                DateOfBirth = new DateOnly(1966, 12, 31),
                Gender = "F",
                Address = "1 rue de Paris",
                PhoneNumber = "0102030405"
            },
            new Patient
            {
                FirstName = "Test",
                LastName = "TestBorderline",
                DateOfBirth = new DateOnly(1945, 6, 24),
                Gender = "M"
            },
            new Patient
            {
                FirstName = "Test",
                LastName = "TestInDanger",
                DateOfBirth = new DateOnly(2004, 6, 18),
                Gender = "M"
            },
            new Patient
            {
                FirstName = "Test",
                LastName = "TestEarlyOnset",
                DateOfBirth = new DateOnly(2002, 6, 28),
                Gender = "F"
            }
        ];

        context.Patients.AddRange(patients);

        await context.SaveChangesAsync();
    }
}