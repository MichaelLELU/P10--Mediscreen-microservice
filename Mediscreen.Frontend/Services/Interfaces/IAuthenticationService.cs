using Mediscreen.Frontend.Models;

namespace Mediscreen.Frontend.Services.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResponse?> LoginAsync(
        LoginRequest request);
}