using Microsoft.EntityFrameworkCore;
using PatientService.Api.Models;

namespace PatientService.Api.Data;

public class PatientDbContext(DbContextOptions<PatientDbContext> options)
    : DbContext(options)
{
    public DbSet<Patient> Patients { get; set; }
}