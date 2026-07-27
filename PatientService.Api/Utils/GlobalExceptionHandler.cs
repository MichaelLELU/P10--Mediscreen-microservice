using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PatientService.Api.Utils;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Une erreur inattendue est survenue.");

        ProblemDetails problemDetails = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erreur interne du serveur",
            Detail = "Une erreur est survenue pendant le traitement de la demande."
        };

        httpContext.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}