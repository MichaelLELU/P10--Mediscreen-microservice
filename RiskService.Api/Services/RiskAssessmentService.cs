using System.Text.RegularExpressions;
using RiskService.Api.Clients.Interfaces;
using RiskService.Api.Models;
using RiskService.Api.Services.Interfaces;

namespace RiskService.Api.Services;

public class RiskAssessmentService(
    IPatientApiClient patientApiClient,
    INoteApiClient noteApiClient)
    : IRiskAssessmentService
{
    private static readonly string[] TriggerTerms =
    [
        "Hemoglobin A1C",
        "Microalbumin",
        "Height",
        "Weight",
        "Smoker",
        "Abnormal",
        "Cholesterol",
        "Dizziness",
        "Relapse",
        "Reaction",
        "Antibodies"
    ];

    public async Task<RiskAssessment?> AssessAsync(
        int patientId,
        CancellationToken cancellationToken = default)
    {
        PatientDto? patient =
            await patientApiClient.GetByIdAsync(
                patientId,
                cancellationToken);

        if (patient is null)
        {
            return null;
        }

        IReadOnlyCollection<PatientNoteDto> notes =
            await noteApiClient.GetByPatientIdAsync(
                patientId,
                cancellationToken);

        int age = CalculateAge(patient.DateOfBirth);

        int triggerCount = CountTriggers(notes);

        RiskLevel riskLevel = DetermineRiskLevel(
            age,
            patient.Gender,
            triggerCount);

        return new RiskAssessment
        {
            PatientId = patient.Id,
            PatientName =
                $"{patient.FirstName} {patient.LastName}",

            Age = age,
            TriggerCount = triggerCount,
            RiskLevel = riskLevel
        };
    }

    private static int CountTriggers(
        IEnumerable<PatientNoteDto> notes)
    {
        int count = 0;

        foreach (PatientNoteDto note in notes)
        {
            foreach (string triggerTerm in TriggerTerms)
            {
                count += Regex.Matches(
                    note.Content,
                    Regex.Escape(triggerTerm),
                    RegexOptions.IgnoreCase |
                    RegexOptions.CultureInvariant).Count;
            }
        }

        return count;
    }

    private static RiskLevel DetermineRiskLevel(
        int age,
        string gender,
        int triggerCount)
    {
        if (triggerCount == 0)
        {
            return RiskLevel.None;
        }

        bool isMale = gender.Equals(
            "M",
            StringComparison.OrdinalIgnoreCase);

        bool isFemale = gender.Equals(
            "F",
            StringComparison.OrdinalIgnoreCase);

        if (age < 30)
        {
            if (isMale && triggerCount >= 5)
            {
                return RiskLevel.EarlyOnset;
            }

            if (isFemale && triggerCount >= 7)
            {
                return RiskLevel.EarlyOnset;
            }

            if (isMale && triggerCount >= 3)
            {
                return RiskLevel.InDanger;
            }

            if (isFemale && triggerCount >= 4)
            {
                return RiskLevel.InDanger;
            }
        }

        if (age >= 30)
        {
            if (triggerCount >= 8)
            {
                return RiskLevel.EarlyOnset;
            }

            if (triggerCount >= 6)
            {
                return RiskLevel.InDanger;
            }

            if (triggerCount >= 2)
            {
                return RiskLevel.Borderline;
            }
        }

        return RiskLevel.None;
    }

    private static int CalculateAge(DateOnly dateOfBirth)
    {
        DateOnly today =
            DateOnly.FromDateTime(DateTime.Today);

        int age = today.Year - dateOfBirth.Year;

        if (dateOfBirth > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}