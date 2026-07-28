using Mediscreen.Frontend.Models;

namespace Mediscreen.Frontend.Services.Interfaces;

public interface IPatientService
{
    Task<IEnumerable<PatientViewModel>> GetAllAsync();
    Task<PatientViewModel?> GetByIdAsync(int id);
    Task<PatientViewModel?> CreateAsync(PatientViewModel patient);
    Task<bool> UpdateAsync(PatientViewModel patient);
}