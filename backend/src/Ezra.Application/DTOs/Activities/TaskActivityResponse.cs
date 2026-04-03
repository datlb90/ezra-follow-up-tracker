using Ezra.Domain.Enums;

namespace Ezra.Application.DTOs.Activities;

public class TaskActivityResponse
{
    public Guid Id { get; init; }
    public Guid FollowUpTaskId { get; init; }
    public DateTime OccurredAt { get; init; }
    public ActivityType Type { get; init; }
    public string Summary { get; init; } = string.Empty;
}
