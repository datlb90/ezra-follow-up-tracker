using Ezra.Application.Interfaces;
using Ezra.Domain.Entities;
using Ezra.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace Ezra.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;

    public ReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Report>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Include(r => r.Findings)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Report?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Include(r => r.Findings)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Finding>> GetFindingsByReportIdAsync(Guid reportId, CancellationToken cancellationToken = default)
    {
        return await _context.Findings
            .Where(f => f.ReportId == reportId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> FindingExistsAsync(Guid findingId, CancellationToken cancellationToken = default)
    {
        return await _context.Findings.AnyAsync(f => f.Id == findingId, cancellationToken);
    }
}
