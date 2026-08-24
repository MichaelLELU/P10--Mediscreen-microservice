namespace Mediscreen.Frontend.Models;

public class PatientDetailsViewModel
{
    public PatientViewModel Patient { get; set; } = new();

    public IReadOnlyList<PatientNoteViewModel> Notes { get; set; } = [];

    public CreatePatientNoteViewModel NewNote { get; set; } = new();
}