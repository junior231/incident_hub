namespace IncidentHub.Api.Domain;

public enum IncidentSeverity { Low = 1, Medium = 2, High = 3, Critical = 4 }
public enum IncidentStatus { Open = 1, Acknowledged = 2, Resolved = 3 }

public class Incident
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public IncidentSeverity Severity { get; set; } = IncidentSeverity.Low;
    public IncidentStatus Status { get; set; } = IncidentStatus.Open;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? AcknowledgedAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public string? Assignee { get; set; }
}
