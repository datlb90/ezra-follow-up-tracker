namespace Ezra.Application.DTOs.Reports;

public class FindingResponse
{
    public Guid Id { get; init; }
    public Guid ReportId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
