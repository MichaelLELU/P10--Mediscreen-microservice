using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiskService.Api.Models;
using RiskService.Api.Services.Interfaces;

namespace RiskService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RiskController(
    IRiskAssessmentService riskAssessmentService)
    : ControllerBase
{
    [HttpGet("{patientId:int}")]
    [ProducesResponseType<RiskAssessment>(
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RiskAssessment>> GetAssessment(
        int patientId,
        CancellationToken cancellationToken)
    {
        RiskAssessment? assessment =
            await riskAssessmentService.AssessAsync(
                patientId,
                cancellationToken);

        if (assessment is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Patient introuvable",
                Detail =
                    $"Aucun patient avec l'identifiant {patientId} n'a été trouvé.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(assessment);
    }
}