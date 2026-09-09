namespace RiskService.Api.Models;

public class PatientDto
{
    public int Id { get; set; }

    public string LastName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public DateOnly DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }
}