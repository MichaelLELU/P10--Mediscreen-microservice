using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using NoteService.Api.Data;
using NoteService.Api.Models;
using NoteService.Api.Repositories.Interfaces;

namespace NoteService.Api.Repositories;

public class PatientNoteRepository(
    NoteDbContext context)
    : IPatientNoteRepository
{
    public async Task<IReadOnlyList<PatientNote>>
        GetByPatientIdAsync(int patientId)
    {
        return await context.Notes
            .AsNoTracking()
            .Where(note => note.PatientId == patientId)
            .OrderBy(note => note.CreatedAt)
            .ToListAsync();
    }

    public async Task<PatientNote?> GetByIdAsync(
        string id)
    {
        return await context.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(note => note.Id == id);
    }

    public async Task<PatientNote> AddAsync(
        PatientNote note)
    {
        note.Id = ObjectId.GenerateNewId().ToString();
        note.CreatedAt = DateTime.UtcNow;

        await context.Notes.AddAsync(note);
        await context.SaveChangesAsync();

        return note;
    }

    public async Task<PatientNote?> UpdateAsync(
        string id,
        string content)
    {
        PatientNote? existingNote =
            await context.Notes
                .FirstOrDefaultAsync(note => note.Id == id);

        if (existingNote is null)
        {
            return null;
        }

        existingNote.Content = content;
        existingNote.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return existingNote;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        PatientNote? existingNote =
            await context.Notes
                .FirstOrDefaultAsync(note => note.Id == id);

        if (existingNote is null)
        {
            return false;
        }

        context.Notes.Remove(existingNote);
        await context.SaveChangesAsync();

        return true;
    }
}