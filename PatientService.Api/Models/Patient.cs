using System.ComponentModel.DataAnnotations;

namespace PatientService.Api.Models;

public class Patient
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public DateOnly DateOfBirth { get; set; }

    [Required]
    [MaxLength(1)]
    public string Gender { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Address { get; set; }

    [MaxLength(30)]
    public string? PhoneNumber { get; set; }
}