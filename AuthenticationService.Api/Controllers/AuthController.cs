using AuthenticationService.Api.Models;
using AuthenticationService.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request)
    {
        ApplicationUser? user =
            await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Adresse e-mail ou mot de passe incorrect."
            });
        }

        bool isPasswordValid =
            await userManager.CheckPasswordAsync(
                user,
                request.Password);

        if (!isPasswordValid)
        {
            return Unauthorized(new
            {
                message = "Adresse e-mail ou mot de passe incorrect."
            });
        }

        LoginResponse response =
            tokenService.CreateToken(user);

        return Ok(response);
    }
}