// File: Contracts/IncidentDtos.cs
using IncidentHub.Api.Domain;

namespace IncidentHub.Api.Contracts;

public record IncidentDto(
    Guid Id,
    string Title,
    string? Description,
    IncidentSeverity Severity,
    IncidentStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? AcknowledgedAt,
    DateTimeOffset? ResolvedAt,
    string? Assignee
);

public record CreateIncidentRequest(
    string Title,
    string? Description,
    IncidentSeverity Severity,
    string? Assignee
);

public record UpdateIncidentRequest(
    string? Title,
    string? Description,
    IncidentSeverity? Severity,
    IncidentStatus? Status,
    string? Assignee
);
