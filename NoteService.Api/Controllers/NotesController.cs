using Microsoft.AspNetCore.Mvc;
using NoteService.Api.Models;
using NoteService.Api.Repositories.Interfaces;

namespace NoteService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController(
    IPatientNoteRepository noteRepository)
    : ControllerBase
{
    [HttpGet("patient/{patientId:int}")]
    public async Task<ActionResult<
        IEnumerable<PatientNote>>> GetByPatientId(
            int patientId)
    {
        IReadOnlyList<PatientNote> notes =
            await noteRepository
                .GetByPatientIdAsync(patientId);

        return Ok(notes);
    }

    [HttpPost]
    public async Task<ActionResult<PatientNote>> Create(
        CreatePatientNoteRequest request)
    {
        PatientNote note = new()
        {
            PatientId = request.PatientId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        PatientNote createdNote =
            await noteRepository.AddAsync(note);

        return CreatedAtAction(
            nameof(GetByPatientId),
            new { patientId = createdNote.PatientId },
            createdNote);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientNote>> GetById(
    string id)
    {
        PatientNote? note =
            await noteRepository.GetByIdAsync(id);

        if (note is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Note introuvable",
                Detail = $"Aucune note ne possède l'identifiant {id}."
            });
        }

        return Ok(note);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PatientNote>> Update(
        string id,
        UpdatePatientNoteRequest request)
    {
        PatientNote? updatedNote =
            await noteRepository.UpdateAsync(
                id,
                request.Content);

        if (updatedNote is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Note introuvable",
                Detail = $"La note {id} ne peut pas être modifiée car elle n'existe pas."
            });
        }

        return Ok(updatedNote);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        bool deleted =
            await noteRepository.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Note introuvable",
                Detail = $"La note {id} ne peut pas être supprimée car elle n'existe pas."
            });
        }

        return NoContent();
    }
}