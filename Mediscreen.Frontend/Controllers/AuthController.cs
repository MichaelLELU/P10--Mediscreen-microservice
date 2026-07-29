using System.Security.Claims;
using Mediscreen.Frontend.Models;
using Mediscreen.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using FrontendAuthenticationService =
    Mediscreen.Frontend.Services.Interfaces.IAuthenticationService;

namespace Mediscreen.Frontend.Controllers;

public class AuthController(
    FrontendAuthenticationService authenticationService) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(
                "Index",
                "Patients");
        }

        ViewData["ReturnUrl"] = returnUrl;

        return View(new LoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        LoginResponse? response;

        try
        {
            response =
                await authenticationService.LoginAsync(
                    new LoginRequest
                    {
                        Username = model.Username,
                        Password = model.Password
                    });
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Impossible de contacter le service d'authentification.");

            return View(model);
        }

        if (response is null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Identifiant ou mot de passe incorrect.");

            return View(model);
        }

        HttpContext.Session.SetString(
            "AccessToken",
            response.Token);

        Claim[] claims =
        [
            new Claim(
                ClaimTypes.Name,
                model.Username)
        ];

        ClaimsIdentity identity = new(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        ClaimsPrincipal principal = new(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = new DateTimeOffset(response.Expiration)
            });

        if (!string.IsNullOrWhiteSpace(returnUrl)
            && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction(
            "Index",
            "Patients");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();

        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(Login));
    }
}