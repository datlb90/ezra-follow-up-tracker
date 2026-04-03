using Ezra.Domain.Enums;

namespace Ezra.Domain.Entities;

public class TaskActivity
{
    public Guid Id { get; set; }
    public Guid FollowUpTaskId { get; set; }
    public DateTime OccurredAt { get; set; }
    public ActivityType Type { get; set; }
    public string Summary { get; set; } = string.Empty;

    public FollowUpTask FollowUpTask { get; set; } = null!;
}
