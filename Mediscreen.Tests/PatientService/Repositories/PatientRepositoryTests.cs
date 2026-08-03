using Microsoft.EntityFrameworkCore;
using PatientService.Api.Data;
using PatientService.Api.Models;
using PatientService.Api.Repositories;

namespace Mediscreen.Tests.PatientService.Repositories;

public class PatientRepositoryTests
{
    private static PatientDbContext CreateContext()
    {
        DbContextOptions<PatientDbContext> options =
            new DbContextOptionsBuilder<PatientDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new PatientDbContext(options);
    }

    private static Patient CreatePatient(
        int id = 0,
        string firstName = "John",
        string lastName = "Doe")
    {
        return new Patient
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = new DateOnly(1980, 1, 1),
            Gender = "M",
            Address = "1 rue de Paris",
            PhoneNumber = "0102030405"
        };
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPatients()
    {
        // Arrange
        await using PatientDbContext context = CreateContext();

        context.Patients.AddRange(
            CreatePatient(firstName: "John", lastName: "Doe"),
            CreatePatient(firstName: "Jane", lastName: "Smith"));

        await context.SaveChangesAsync();

        PatientRepository repository = new(context);

        // Act
        IEnumerable<Patient> result =
            await repository.GetAllAsync();

        // Assert
        List<Patient> patients = result.ToList();

        Assert.Equal(2, patients.Count);
        Assert.Contains(
            patients,
            patient => patient.FirstName == "John");
        Assert.Contains(
            patients,
            patient => patient.FirstName == "Jane");
    }

    [Fact]
    public async Task GetAllAsync_WhenNoPatientExists_ShouldReturnEmptyCollection()
    {
        // Arrange
        await using PatientDbContext context = CreateContext();

        PatientRepository repository = new(context);

        // Act
        IEnumerable<Patient> result =
            await repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPatientExists_ShouldReturnPatient()
    {
        // Arrange
        await using PatientDbContext context = CreateContext();

        Patient patient = CreatePatient(
            firstName: "John",
            lastName: "Doe");

        context.Patients.Add(patient);
        await context.SaveChangesAsync();

        PatientRepository repository = new(context);

        // Act
        Patient? result =
            await repository.GetByIdAsync(patient.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(patient.Id, result.Id);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPatientDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using PatientDbContext context = CreateContext();

        PatientRepository repository = new(context);

        // Act
        Patient? result =
            await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldSaveAndReturnPatient()
    {
        // Arrange
        await using PatientDbContext context = CreateContext();

        PatientRepository repository = new(context);
        Patient patient = CreatePatient();

        // Act
        Patient result =
            await repository.AddAsync(patient);

        // Assert
        Assert.True(result.Id > 0);

        Patient? savedPatient =
            await context.Patients.FindAsync(result.Id);

        Assert.NotNull(savedPatient);
        Assert.Equal("John", savedPatient.FirstName);
        Assert.Equal("Doe", savedPatient.LastName);
    }

    [Fact]
    public async Task UpdateAsync_WhenPatientExists_ShouldUpdateAllProperties()
    {
        // Arrange
        await using PatientDbContext context = CreateContext();

        Patient existingPatient = CreatePatient(
            firstName: "John",
            lastName: "Doe");

        context.Patients.Add(existingPatient);
        await context.SaveChangesAsync();

        PatientRepository repository = new(context);

        Patient updatedPatient = new()
        {
            Id = existingPatient.Id,
            FirstName = "Michael",
            LastName = "Martin",
            DateOfBirth = new DateOnly(1990, 5, 20),
            Gender = "M",
            Address = "10 avenue de Lyon",
            PhoneNumber = "0607080910"
        };

        // Act
        bool result =
            await repository.UpdateAsync(updatedPatient);

        // Assert
        Assert.True(result);

        Patient? savedPatient =
            await context.Patients.FindAsync(existingPatient.Id);

        Assert.NotNull(savedPatient);
        Assert.Equal("Michael", savedPatient.FirstName);
        Assert.Equal("Martin", savedPatient.LastName);
        Assert.Equal(
            new DateOnly(1990, 5, 20),
            savedPatient.DateOfBirth);
        Assert.Equal("M", savedPatient.Gender);
        Assert.Equal(
            "10 avenue de Lyon",
            savedPatient.Address);
        Assert.Equal(
            "0607080910",
            savedPatient.PhoneNumber);
    }

    [Fact]
    public async Task UpdateAsync_WhenPatientDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        await using PatientDbContext context = CreateContext();

        PatientRepository repository = new(context);
        Patient patient = CreatePatient(id: 999);

        // Act
        bool result =
            await repository.UpdateAsync(patient);

        // Assert
        Assert.False(result);
        Assert.Empty(context.Patients);
    }
}