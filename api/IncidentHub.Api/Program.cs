using IncidentHub.Api.Contracts;
using IncidentHub.Api.Domain;
using IncidentHub.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---- Database connection (Postgres) ----
// Use environment variable when deployed; fallback to local dev string
var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=incidenthub;Username=incident;Password=incidentpw";

// Log connection string (masking password) for debugging
try
{
    var safeLog = connStr;
    if (safeLog.Contains("Password="))
    {
        var parts = safeLog.Split(';')
            .Select(p => p.StartsWith("Password=", StringComparison.OrdinalIgnoreCase) ? "Password=***" : p);
        safeLog = string.Join(";", parts);
    }
    Console.WriteLine($"[DB CONFIG] Using connection string: {safeLog}");
}
catch (Exception ex)
{
    Console.WriteLine($"[DB CONFIG] Failed to parse connection string for logging: {ex.Message}");
}

// Configure EF with Npgsql
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(connStr, npgsqlOptions =>
    {
        // Optionally set keepalive so Render/Supabase connections stay alive
        npgsqlOptions.CommandTimeout(30);
    }).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

// ---- Swagger (API docs) ----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "IncidentHub API",
        Version = "v1"
    });
});

// ---- CORS (allow your Next.js dev server locally) ----
builder.Services.AddCors(o =>
{
    o.AddPolicy("app", p =>
        p.WithOrigins(
            "http://localhost:3000",
            "http://127.0.0.1:3000"
        )
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// ---- Ensure we listen on Render's dynamic PORT ----
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

// ---- Middleware ----
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IncidentHub API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("app");

app.MapGet("/", () => Results.Redirect("/swagger"));

// ---------------- Incidents CRUD ----------------

// List incidents with optional filters + paging
app.MapGet("/incidents", async (AppDbContext db, int page = 1, int pageSize = 20) =>
{
    if (page < 1) page = 1;
    if (pageSize is < 1 or > 100) pageSize = 20;

    var q = db.Incidents.AsQueryable()
        .OrderByDescending(i => i.CreatedAt);

    var total = await q.CountAsync();
    var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

    var dtos = items.Select(i => new IncidentDto(
        i.Id, i.Title, i.Description, i.Severity, i.Status,
        i.Assignee, i.CreatedAt, i.AcknowledgedAt, i.ResolvedAt));

    return Results.Ok(new { total, page, pageSize, items = dtos });
});

// Get one
app.MapGet("/incidents/{id:guid}", async (AppDbContext db, Guid id) =>
{
    var i = await db.Incidents.FindAsync(id);
    return i is null
        ? Results.NotFound()
        : Results.Ok(new IncidentDto(i.Id, i.Title, i.Description, i.Severity, i.Status,
            i.Assignee, i.CreatedAt, i.AcknowledgedAt, i.ResolvedAt));
});

// Create
app.MapPost("/incidents", async (AppDbContext db, CreateIncidentDto req) =>
{
    if (string.IsNullOrWhiteSpace(req.Title)) return Results.BadRequest("Title is required");

    var entity = new Incident
    {
        Title = req.Title.Trim(),
        Description = req.Description?.Trim(),
        Severity = req.Severity,
        Status = req.Status,
        Assignee = string.IsNullOrWhiteSpace(req.Assignee) ? null : req.Assignee!.Trim()
    };

    db.ChangeTracker.Clear();
    db.Incidents.Add(entity);
    await db.SaveChangesAsync();

    return Results.Created($"/incidents/{entity.Id}", new { id = entity.Id });
});

// Update (sets acknowledged/resolved timestamps)
app.MapPut("/incidents/{id:guid}", async (AppDbContext db, Guid id, UpdateIncidentDto req) =>
{
    var i = await db.Incidents.AsTracking().FirstOrDefaultAsync(x => x.Id == id);
    if (i is null) return Results.NotFound();

    if (req.Title is not null) i.Title = req.Title.Trim();
    if (req.Description is not null) i.Description = req.Description.Trim();
    if (req.Severity.HasValue) i.Severity = req.Severity.Value;
    if (req.Status.HasValue)
    {
        i.Status = req.Status.Value;
        if (i.Status == StatusType.Acknowledged && i.AcknowledgedAt is null)
            i.AcknowledgedAt = DateTime.UtcNow;
        if (i.Status == StatusType.Resolved && i.ResolvedAt is null)
            i.ResolvedAt = DateTime.UtcNow;
    }
    if (req.Assignee is not null) i.Assignee = string.IsNullOrWhiteSpace(req.Assignee) ? null : req.Assignee.Trim();

    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Delete
app.MapDelete("/incidents/{id:guid}", async (AppDbContext db, Guid id) =>
{
    var i = await db.Incidents.FindAsync(id);
    if (i is null) return Results.NotFound();
    db.Remove(i);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
