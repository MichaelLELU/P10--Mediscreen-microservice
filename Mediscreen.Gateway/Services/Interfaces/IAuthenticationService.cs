using Mediscreen.Gateway.Models;

namespace Mediscreen.Gateway.Services.Interfaces;

public interface IAuthenticationService
{
    LoginResponse? Login(LoginRequest request);
}