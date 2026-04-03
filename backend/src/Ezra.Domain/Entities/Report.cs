namespace Ezra.Domain.Entities;

public class Report
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }

    public ICollection<Finding> Findings { get; set; } = [];
}
