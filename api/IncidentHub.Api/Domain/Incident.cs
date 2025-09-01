using System;

namespace IncidentHub.Api.Domain
{
    public enum SeverityLevel
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }

    public enum StatusType
    {
        Open = 0,
        Acknowledged = 1,
        Resolved = 2
    }

    public class Incident
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Enum-based fields
        public SeverityLevel Severity { get; set; } = SeverityLevel.Low;
        public StatusType Status { get; set; } = StatusType.Open;

        public string? Assignee { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AcknowledgedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
