using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using IncidentHub.Api.Infrastructure;

namespace IncidentHub.Api.Infrastructure
{
    // Needed so EF Core CLI knows how to create AppDbContext at design-time
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Default to localhost dev connection if nothing passed
            var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? "Host=localhost;Port=5432;Database=incidenthub;Username=incident;Password=incidentpw";

            // Allow --connection override from EF CLI
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connStr, npgsqlOptions =>
            {
                npgsqlOptions.CommandTimeout(30);
                npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
