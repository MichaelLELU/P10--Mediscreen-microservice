using Mediscreen.Frontend.Models;
using Mediscreen.Frontend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mediscreen.Frontend.Controllers;

[Authorize]
public class NotesController(
    INoteService noteService) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind(Prefix = "NewNote")]
    CreatePatientNoteViewModel note)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] =
                "Le contenu de la note est obligatoire.";

            return RedirectToPatient(note.PatientId);
        }

        try
        {
            await noteService.CreateAsync(note);

            TempData["SuccessMessage"] =
                "La note a bien été ajoutée.";
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] =
                "Impossible d'ajouter la note.";
        }

        return RedirectToPatient(note.PatientId);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(
        UpdatePatientNoteViewModel note)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] =
                "Le contenu de la note est obligatoire.";

            return RedirectToPatient(note.PatientId);
        }

        try
        {
            PatientNoteViewModel? updatedNote =
                await noteService.UpdateAsync(
                    note.Id,
                    note);

            if (updatedNote is null)
            {
                TempData["ErrorMessage"] =
                    "La note est introuvable.";
            }
            else
            {
                TempData["SuccessMessage"] =
                    "La note a bien été modifiée.";
            }
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] =
                "Impossible de modifier la note.";
        }

        return RedirectToPatient(note.PatientId);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        string id,
        int patientId)
    {
        try
        {
            bool deleted =
                await noteService.DeleteAsync(id);

            if (deleted)
            {
                TempData["SuccessMessage"] =
                    "La note a bien été supprimée.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    "La note est introuvable.";
            }
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] =
                "Impossible de supprimer la note.";
        }

        return RedirectToPatient(patientId);
    }

    private IActionResult RedirectToPatient(
        int patientId)
    {
        return RedirectToAction(
            nameof(PatientsController.Details),
            "Patients",
            new { id = patientId });
    }
}