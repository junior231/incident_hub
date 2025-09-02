using IncidentHub.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace IncidentHub.Api.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Incident> Incidents => Set<Incident>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Incident>(e =>
        {
            // Explicitly map to the "Incidents" table (case-sensitive in Postgres)
            e.ToTable("Incidents");

            e.HasKey(x => x.Id);
            e.Property(x => x.Title).IsRequired().HasMaxLength(240);
            e.Property(x => x.Description).HasMaxLength(4000);
            e.Property(x => x.Severity).HasConversion<int>();
            e.Property(x => x.Status).HasConversion<int>();
            e.HasIndex(x => new { x.Severity, x.Status });
        });
    }
}
