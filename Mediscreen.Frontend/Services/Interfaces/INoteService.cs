using Mediscreen.Frontend.Models;

namespace Mediscreen.Frontend.Services.Interfaces;

public interface INoteService
{
    Task<IReadOnlyList<PatientNoteViewModel>>
        GetByPatientIdAsync(int patientId);

    Task<PatientNoteViewModel?> CreateAsync(
        CreatePatientNoteViewModel request);

    Task<PatientNoteViewModel?> UpdateAsync(
        string id,
        UpdatePatientNoteViewModel request);

    Task<bool> DeleteAsync(string id);
}