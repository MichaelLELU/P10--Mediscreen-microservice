using AuthenticationService.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Api.Data;

public class AuthenticationDbContext(
    DbContextOptions<AuthenticationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
}