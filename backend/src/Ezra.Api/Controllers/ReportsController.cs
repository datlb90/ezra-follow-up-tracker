using Ezra.Application.DTOs.Reports;
using Ezra.Application.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ezra.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ReportResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var reports = await _reportService.GetReportsAsync(cancellationToken);
        return Ok(reports);
    }

    [HttpGet("{reportId:guid}/findings")]
    [ProducesResponseType(typeof(IReadOnlyList<FindingResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFindings(Guid reportId, CancellationToken cancellationToken)
    {
        var findings = await _reportService.GetReportFindingsAsync(reportId, cancellationToken);
        return Ok(findings);
    }
}
