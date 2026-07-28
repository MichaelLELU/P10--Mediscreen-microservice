using System.ComponentModel.DataAnnotations;

namespace Mediscreen.Frontend.Models;

public class PatientViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    [Display(Name = "Nom")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prénom est obligatoire.")]
    [Display(Name = "Prénom")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La date de naissance est obligatoire.")]
    [DataType(DataType.Date)]
    [Display(Name = "Date de naissance")]
    public DateOnly DateOfBirth { get; set; }

    [Required(ErrorMessage = "Le genre est obligatoire.")]
    [Display(Name = "Genre")]
    public string Gender { get; set; } = string.Empty;

    [Display(Name = "Adresse")]
    public string? Address { get; set; }

    [Display(Name = "Téléphone")]
    public string? PhoneNumber { get; set; }
}