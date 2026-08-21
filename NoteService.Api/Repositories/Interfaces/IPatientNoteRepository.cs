using NoteService.Api.Models;

namespace NoteService.Api.Repositories.Interfaces;

public interface IPatientNoteRepository
{
    Task<IReadOnlyList<PatientNote>>
        GetByPatientIdAsync(int patientId);

    Task<PatientNote?> GetByIdAsync(string id);

    Task<PatientNote> AddAsync(PatientNote note);

    Task<PatientNote?> UpdateAsync(
        string id,
        string content);

    Task<bool> DeleteAsync(string id);
}