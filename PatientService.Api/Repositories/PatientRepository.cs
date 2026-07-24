using Microsoft.EntityFrameworkCore;
using PatientService.Api.Data;
using PatientService.Api.Models;
using PatientService.Api.Repositories.Interfaces;

namespace PatientService.Api.Repositories;

public class PatientRepository(PatientDbContext context) : IPatientRepository
{
    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        return await context.Patients
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Patient?> GetByIdAsync(int id)
    {
        return await context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(patient => patient.Id == id);
    }

    public async Task<Patient> AddAsync(Patient patient)
    {
        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        return patient;
    }

    public async Task<bool> UpdateAsync(Patient patient)
    {
        Patient? existingPatient =
            await context.Patients.FindAsync(patient.Id);

        if (existingPatient is null)
        {
            return false;
        }

        existingPatient.FirstName = patient.FirstName;
        existingPatient.LastName = patient.LastName;
        existingPatient.DateOfBirth = patient.DateOfBirth;
        existingPatient.Gender = patient.Gender;
        existingPatient.Address = patient.Address;
        existingPatient.PhoneNumber = patient.PhoneNumber;

        await context.SaveChangesAsync();

        return true;
    }
}