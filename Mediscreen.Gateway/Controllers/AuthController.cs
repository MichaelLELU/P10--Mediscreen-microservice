using Mediscreen.Gateway.Models;
using Mediscreen.Gateway.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mediscreen.Gateway.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(
        IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(
        [FromBody] LoginRequest request)
    {
        LoginResponse? response =
            _authenticationService.Login(request);

        if (response is null)
        {
            return Unauthorized(new
            {
                message = "Identifiant ou mot de passe incorrect."
            });
        }

        return Ok(response);
    }
}