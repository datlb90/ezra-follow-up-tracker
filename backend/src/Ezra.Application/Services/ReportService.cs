using Ezra.Application.DTOs.Reports;
using Ezra.Application.Interfaces;

namespace Ezra.Application.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    public ReportService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<IReadOnlyList<ReportResponse>> GetReportsAsync(CancellationToken cancellationToken = default)
    {
        var reports = await _reportRepository.GetAllAsync(cancellationToken);

        return reports.Select(r => new ReportResponse
        {
            Id = r.Id,
            Title = r.Title,
            ReceivedAt = r.ReceivedAt,
            FindingCount = r.Findings.Count
        }).ToList();
    }

    public async Task<IReadOnlyList<FindingResponse>> GetReportFindingsAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        var findings = await _reportRepository.GetFindingsByReportIdAsync(reportId, cancellationToken);

        return findings.Select(f => new FindingResponse
        {
            Id = f.Id,
            ReportId = f.ReportId,
            Title = f.Title,
            Description = f.Description
        }).ToList();
    }
}
