using Microsoft.AspNetCore.Mvc;
using PatientService.Api.Models;
using PatientService.Api.Repositories.Interfaces;

namespace PatientService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController(IPatientRepository patientRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Patient>>> GetAll()
    {
        IEnumerable<Patient> patients =
            await patientRepository.GetAllAsync();

        return Ok(patients);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Patient>> GetById(int id)
    {
        Patient? patient =
            await patientRepository.GetByIdAsync(id);

        if (patient is null)
        {
            return NotFound();
        }

        return Ok(patient);
    }

    [HttpPost]
    public async Task<ActionResult<Patient>> Create(Patient patient)
    {
        Patient createdPatient =
            await patientRepository.AddAsync(patient);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdPatient.Id },
            createdPatient);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Patient patient)
    {
        if (id != patient.Id)
        {
            return BadRequest(
                "L'identifiant de l'URL doit correspondre à celui du patient.");
        }

        bool updated = await patientRepository.UpdateAsync(patient);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }
}