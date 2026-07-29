using Mediscreen.Frontend.Models;
using Mediscreen.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mediscreen.Frontend.Controllers;

[Authorize]
public class PatientsController(
    IPatientService patientService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            IEnumerable<PatientViewModel> patients =
                await patientService.GetAllAsync();

            return View(patients);
        }
        catch (HttpRequestException)
        {
            ViewBag.ErrorMessage =
                "Impossible de récupérer les patients. Vérifiez que la Gateway et l'API Patient sont démarrées.";

            return View(Enumerable.Empty<PatientViewModel>());
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new PatientViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        PatientViewModel patient)
    {
        if (!ModelState.IsValid)
        {
            return View(patient);
        }

        try
        {
            await patientService.CreateAsync(patient);

            TempData["SuccessMessage"] =
                "Le patient a bien été ajouté.";

            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Impossible d'ajouter le patient. Vérifiez que les services sont démarrés.");

            return View(patient);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            PatientViewModel? patient =
                await patientService.GetByIdAsync(id);

            if (patient is null)
            {
                return NotFound();
            }

            return View(patient);
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] =
                "Impossible de récupérer le patient.";

            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        PatientViewModel patient)
    {
        if (id != patient.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(patient);
        }

        try
        {
            bool updated =
                await patientService.UpdateAsync(patient);

            if (!updated)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] =
                "Le patient a bien été modifié.";

            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Impossible de modifier le patient. Vérifiez que les services sont démarrés.");

            return View(patient);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            PatientViewModel? patient =
                await patientService.GetByIdAsync(id);

            if (patient is null)
            {
                return NotFound();
            }

            return View(patient);
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] =
                "Impossible de récupérer le détail du patient.";

            return RedirectToAction(nameof(Index));
        }
    }
}