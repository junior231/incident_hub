using IncidentHub.Api.Contracts;
using IncidentHub.Api.Domain;
using IncidentHub.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---- Database connection (Postgres in Docker) ----
var connStr = "Host=localhost;Port=5432;Database=incidenthub;Username=incident;Password=incidentpw";
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(connStr).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

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


// ---- CORS (allow your Next.js dev server) ----
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

// ---- Middleware ----
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IncidentHub API v1");
    c.RoutePrefix = "swagger"; // so UI is at /swagger
});


// Keep dev simple: stay on HTTP to avoid the HTTPS warning
// app.UseHttpsRedirection();

app.UseCors("app");

app.MapGet("/", () => Results.Redirect("/swagger"));

// ---------------- Incidents CRUD ----------------

// List incidents with optional filters + paging (simple version)
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
        i.CreatedAt, i.AcknowledgedAt, i.ResolvedAt, i.Assignee));

    return Results.Ok(new { total, page, pageSize, items = dtos });
});

// Get one
app.MapGet("/incidents/{id:guid}", async (AppDbContext db, Guid id) =>
{
    var i = await db.Incidents.FindAsync(id);
    return i is null
        ? Results.NotFound()
        : Results.Ok(new IncidentDto(i.Id, i.Title, i.Description, i.Severity, i.Status,
            i.CreatedAt, i.AcknowledgedAt, i.ResolvedAt, i.Assignee));
});

// Create
app.MapPost("/incidents", async (AppDbContext db, CreateIncidentRequest req) =>
{
    if (string.IsNullOrWhiteSpace(req.Title)) return Results.BadRequest("Title is required");

    var entity = new Incident
    {
        Title = req.Title.Trim(),
        Description = req.Description?.Trim(),
        Severity = req.Severity,
        Assignee = string.IsNullOrWhiteSpace(req.Assignee) ? null : req.Assignee!.Trim()
    };

    // Since we used NoTracking globally, ensure we track this new entity
    db.ChangeTracker.Clear();
    db.Incidents.Add(entity);
    await db.SaveChangesAsync();

    return Results.Created($"/incidents/{entity.Id}", new { id = entity.Id });
});

// Update (also sets acknowledged/resolved timestamps)
app.MapPut("/incidents/{id:guid}", async (AppDbContext db, Guid id, UpdateIncidentRequest req) =>
{
    var i = await db.Incidents.AsTracking().FirstOrDefaultAsync(x => x.Id == id);
    if (i is null) return Results.NotFound();

    if (req.Title is not null) i.Title = req.Title.Trim();
    if (req.Description is not null) i.Description = req.Description.Trim();
    if (req.Severity.HasValue) i.Severity = req.Severity.Value;
    if (req.Status.HasValue)
    {
        i.Status = req.Status.Value;
        if (i.Status == IncidentStatus.Acknowledged && i.AcknowledgedAt is null)
            i.AcknowledgedAt = DateTimeOffset.UtcNow;
        if (i.Status == IncidentStatus.Resolved && i.ResolvedAt is null)
            i.ResolvedAt = DateTimeOffset.UtcNow;
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
