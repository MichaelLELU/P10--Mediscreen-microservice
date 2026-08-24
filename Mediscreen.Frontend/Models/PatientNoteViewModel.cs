namespace Mediscreen.Frontend.Models;

public class PatientNoteViewModel
{
    public string Id { get; set; } = string.Empty;

    public int PatientId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}