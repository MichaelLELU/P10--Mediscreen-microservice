using PatientService.Api.Models;

namespace PatientService.Api.Repositories.Interfaces;

public interface IPatientRepository
{
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(int id);
    Task<Patient> AddAsync(Patient patient);
    Task<bool> UpdateAsync(Patient patient);
}