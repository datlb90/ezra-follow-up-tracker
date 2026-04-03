namespace Ezra.Application.DTOs.Reports;

public class ReportResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateTime ReceivedAt { get; init; }
    public int FindingCount { get; init; }
}
