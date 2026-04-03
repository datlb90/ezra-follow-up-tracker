using Ezra.Domain.Enums;

namespace Ezra.Domain.Entities;

public class FollowUpTask
{
    public Guid Id { get; set; }
    public Guid FindingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public FollowUpTaskStatus Status { get; set; } = FollowUpTaskStatus.NotStarted;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Finding Finding { get; set; } = null!;
    public ICollection<TaskActivity> Activities { get; set; } = [];
}
