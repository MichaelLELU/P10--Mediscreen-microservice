using System.ComponentModel.DataAnnotations;

namespace NoteService.Api.Models;

public class UpdatePatientNoteRequest
{
    [Required]
    public string Content { get; set; } = string.Empty;
}