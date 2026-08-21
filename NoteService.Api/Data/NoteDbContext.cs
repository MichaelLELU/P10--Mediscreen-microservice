using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using NoteService.Api.Models;

namespace NoteService.Api.Data;

public class NoteDbContext(
    DbContextOptions<NoteDbContext> options)
    : DbContext(options)
{
    public DbSet<PatientNote> Notes { get; init; } = null!;

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PatientNote>(entity =>
        {
            entity.ToCollection("Notes");

            entity.HasIndex(note => note.PatientId);
        });
    }
}