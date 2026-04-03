namespace Ezra.Domain.Entities;

public class Finding
{
    public Guid Id { get; set; }
    public Guid ReportId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Report Report { get; set; } = null!;
    public ICollection<FollowUpTask> FollowUpTasks { get; set; } = [];
}
