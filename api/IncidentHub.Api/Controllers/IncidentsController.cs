using IncidentHub.Api.Domain;
using IncidentHub.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IncidentHub.Api.Controllers
{
    [ApiController]
    [Route("incidents")]
    public class IncidentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IncidentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /incidents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Incident>>> GetIncidents()
        {
            return await _context.Incidents
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        // POST /incidents
        [HttpPost]
        public async Task<ActionResult<Incident>> CreateIncident([FromBody] Incident incident)
        {
            if (string.IsNullOrWhiteSpace(incident.Title))
                return BadRequest("Title is required");

            incident.Id = Guid.NewGuid();
            incident.CreatedAt = DateTime.UtcNow;

            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIncidents), new { id = incident.Id }, incident);
        }
    }
}
