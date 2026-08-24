using System.ComponentModel.DataAnnotations;

namespace Mediscreen.Frontend.Models;

public class UpdatePatientNoteViewModel
{
    public string Id { get; set; } = string.Empty;

    public int PatientId { get; set; }

    [Required(ErrorMessage = "Le contenu de la note est obligatoire.")]
    public string Content { get; set; } = string.Empty;
}