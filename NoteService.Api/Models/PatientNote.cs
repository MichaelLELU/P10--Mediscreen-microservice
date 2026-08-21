using System.ComponentModel.DataAnnotations;

namespace NoteService.Api.Models;

public class PatientNote
{
    [Key]
    public string Id { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int PatientId { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}