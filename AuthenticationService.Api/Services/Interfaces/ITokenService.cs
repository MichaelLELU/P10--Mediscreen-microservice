using AuthenticationService.Api.Models;

namespace AuthenticationService.Api.Services.Interfaces;

public interface ITokenService
{
    LoginResponse CreateToken(ApplicationUser user);
}