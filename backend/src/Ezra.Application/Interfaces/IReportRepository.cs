using Ezra.Domain.Entities;

namespace Ezra.Application.Interfaces;

public interface IReportRepository
{
    Task<IReadOnlyList<Report>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Report?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Finding>> GetFindingsByReportIdAsync(Guid reportId, CancellationToken cancellationToken = default);
}
