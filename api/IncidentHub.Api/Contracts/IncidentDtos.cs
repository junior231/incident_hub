// File: Contracts/IncidentDtos.cs
using IncidentHub.Api.Domain;

namespace IncidentHub.Api.Contracts;

 public record IncidentDto(
        Guid Id,
        string Title,
        string? Description,
        SeverityLevel Severity,
        StatusType Status,
        string? Assignee,
        DateTime CreatedAt,
        DateTime? AcknowledgedAt,
        DateTime? ResolvedAt
    );

 public record CreateIncidentDto(
        string Title,
        string? Description,
        SeverityLevel Severity,
        StatusType Status,
        string? Assignee
    );

public record UpdateIncidentDto(
    string? Title,
    string? Description,
    SeverityLevel? Severity,
    StatusType? Status,
    string? Assignee
);
