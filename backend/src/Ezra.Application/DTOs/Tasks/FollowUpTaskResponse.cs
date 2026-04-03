using Ezra.Domain.Enums;

namespace Ezra.Application.DTOs.Tasks;

public class FollowUpTaskResponse
{
    public Guid Id { get; init; }
    public Guid FindingId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public FollowUpTaskStatus Status { get; init; }
    public DateTime? DueAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public int PriorityScore { get; init; }
    public TaskPriorityLevel PriorityLevel { get; init; }
    public string PriorityReason { get; init; } = string.Empty;
}
