using Ezra.Application.DTOs.Reports;

namespace Ezra.Application.Interfaces;

public interface IReportService
{
    Task<IReadOnlyList<ReportResponse>> GetReportsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FindingResponse>> GetReportFindingsAsync(Guid reportId, CancellationToken cancellationToken = default);
}
