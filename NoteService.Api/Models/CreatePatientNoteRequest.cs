using System.ComponentModel.DataAnnotations;

namespace NoteService.Api.Models;

public class CreatePatientNoteRequest
{
    [Range(1, int.MaxValue)]
    public int PatientId { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;
}