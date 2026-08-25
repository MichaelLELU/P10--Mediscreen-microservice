using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NoteService.Api.Controllers;
using NoteService.Api.Models;
using NoteService.Api.Repositories.Interfaces;

namespace Mediscreen.Tests.NoteService.Controllers;

public class NotesControllerTests
{
    private readonly Mock<IPatientNoteRepository> _repositoryMock =
        new();

    private readonly NotesController _controller;

    public NotesControllerTests()
    {
        _controller =
            new NotesController(_repositoryMock.Object);
    }

    private static PatientNote CreateNote(
        string id = "507f1f77bcf86cd799439011",
        int patientId = 1,
        string content = "Note médicale de test.")
    {
        return new PatientNote
        {
            Id = id,
            PatientId = patientId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task GetByPatientId_ShouldReturnOkWithNotes()
    {
        // Arrange
        List<PatientNote> notes =
        [
            CreateNote(),
            CreateNote(
                id: "507f1f77bcf86cd799439012",
                content: "Deuxième note.")
        ];

        _repositoryMock
            .Setup(repository =>
                repository.GetByPatientIdAsync(1))
            .ReturnsAsync(notes);

        // Act
        ActionResult<IEnumerable<PatientNote>> result =
            await _controller.GetByPatientId(1);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        IReadOnlyList<PatientNote> returnedNotes =
            Assert.IsAssignableFrom<IReadOnlyList<PatientNote>>(
                okResult.Value);

        Assert.Equal(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        Assert.Equal(2, returnedNotes.Count);
    }

    [Fact]
    public async Task GetByPatientId_WhenNoNoteExists_ShouldReturnEmptyList()
    {
        // Arrange
        _repositoryMock
            .Setup(repository =>
                repository.GetByPatientIdAsync(1))
            .ReturnsAsync([]);

        // Act
        ActionResult<IEnumerable<PatientNote>> result =
            await _controller.GetByPatientId(1);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        IReadOnlyList<PatientNote> returnedNotes =
            Assert.IsAssignableFrom<IReadOnlyList<PatientNote>>(
                okResult.Value);

        Assert.Empty(returnedNotes);
    }

    [Fact]
    public async Task GetById_WhenNoteExists_ShouldReturnOk()
    {
        // Arrange
        PatientNote note = CreateNote();

        _repositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(note.Id))
            .ReturnsAsync(note);

        // Act
        ActionResult<PatientNote> result =
            await _controller.GetById(note.Id);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        PatientNote returnedNote =
            Assert.IsType<PatientNote>(okResult.Value);

        Assert.Equal(note.Id, returnedNote.Id);
        Assert.Equal(note.Content, returnedNote.Content);
    }

    [Fact]
    public async Task GetById_WhenNoteDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        const string noteId =
            "507f1f77bcf86cd799439099";

        _repositoryMock
            .Setup(repository =>
                repository.GetByIdAsync(noteId))
            .ReturnsAsync((PatientNote?)null);

        // Act
        ActionResult<PatientNote> result =
            await _controller.GetById(noteId);

        // Assert
        NotFoundObjectResult notFoundResult =
            Assert.IsType<NotFoundObjectResult>(
                result.Result);

        ProblemDetails problem =
            Assert.IsType<ProblemDetails>(
                notFoundResult.Value);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            problem.Status);

        Assert.Equal(
            "Note introuvable",
            problem.Title);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedNote()
    {
        // Arrange
        CreatePatientNoteRequest request = new()
        {
            PatientId = 1,
            Content = "Nouvelle note médicale."
        };

        PatientNote createdNote =
            CreateNote(content: request.Content);

        _repositoryMock
            .Setup(repository =>
                repository.AddAsync(
                    It.Is<PatientNote>(note =>
                        note.PatientId == request.PatientId
                        && note.Content == request.Content)))
            .ReturnsAsync(createdNote);

        // Act
        ActionResult<PatientNote> result =
            await _controller.Create(request);

        // Assert
        CreatedAtActionResult createdResult =
            Assert.IsType<CreatedAtActionResult>(
                result.Result);

        PatientNote returnedNote =
            Assert.IsType<PatientNote>(
                createdResult.Value);

        Assert.Equal(
            nameof(NotesController.GetByPatientId),
            createdResult.ActionName);

        Assert.Equal(
            request.PatientId,
            createdResult.RouteValues?["patientId"]);

        Assert.Equal(
            createdNote.Id,
            returnedNote.Id);

        _repositoryMock.Verify(
            repository =>
                repository.AddAsync(
                    It.IsAny<PatientNote>()),
            Times.Once);
    }

    [Fact]
    public async Task Update_WhenNoteExists_ShouldReturnOk()
    {
        // Arrange
        PatientNote existingNote = CreateNote();

        UpdatePatientNoteRequest request = new()
        {
            Content = "Contenu médical modifié."
        };

        PatientNote updatedNote = CreateNote(
            content: request.Content);

        updatedNote.UpdatedAt = DateTime.UtcNow;

        _repositoryMock
            .Setup(repository =>
                repository.UpdateAsync(
                    existingNote.Id,
                    request.Content))
            .ReturnsAsync(updatedNote);

        // Act
        ActionResult<PatientNote> result =
            await _controller.Update(
                existingNote.Id,
                request);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        PatientNote returnedNote =
            Assert.IsType<PatientNote>(okResult.Value);

        Assert.Equal(
            request.Content,
            returnedNote.Content);

        Assert.NotNull(returnedNote.UpdatedAt);
    }

    [Fact]
    public async Task Update_WhenNoteDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        const string noteId =
            "507f1f77bcf86cd799439099";

        UpdatePatientNoteRequest request = new()
        {
            Content = "Contenu modifié."
        };

        _repositoryMock
            .Setup(repository =>
                repository.UpdateAsync(
                    noteId,
                    request.Content))
            .ReturnsAsync((PatientNote?)null);

        // Act
        ActionResult<PatientNote> result =
            await _controller.Update(
                noteId,
                request);

        // Assert
        NotFoundObjectResult notFoundResult =
            Assert.IsType<NotFoundObjectResult>(
                result.Result);

        ProblemDetails problem =
            Assert.IsType<ProblemDetails>(
                notFoundResult.Value);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            problem.Status);

        Assert.Equal(
            "Note introuvable",
            problem.Title);
    }

    [Fact]
    public async Task Delete_WhenNoteExists_ShouldReturnNoContent()
    {
        // Arrange
        const string noteId =
            "507f1f77bcf86cd799439011";

        _repositoryMock
            .Setup(repository =>
                repository.DeleteAsync(noteId))
            .ReturnsAsync(true);

        // Act
        IActionResult result =
            await _controller.Delete(noteId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _repositoryMock.Verify(
            repository =>
                repository.DeleteAsync(noteId),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenNoteDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        const string noteId =
            "507f1f77bcf86cd799439099";

        _repositoryMock
            .Setup(repository =>
                repository.DeleteAsync(noteId))
            .ReturnsAsync(false);

        // Act
        IActionResult result =
            await _controller.Delete(noteId);

        // Assert
        NotFoundObjectResult notFoundResult =
            Assert.IsType<NotFoundObjectResult>(result);

        ProblemDetails problem =
            Assert.IsType<ProblemDetails>(
                notFoundResult.Value);

        Assert.Equal(
            StatusCodes.Status404NotFound,
            problem.Status);

        Assert.Equal(
            "Note introuvable",
            problem.Title);
    }
}