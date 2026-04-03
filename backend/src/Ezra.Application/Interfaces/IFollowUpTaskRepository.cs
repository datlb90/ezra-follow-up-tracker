using Ezra.Domain.Entities;
using Ezra.Domain.Enums;

namespace Ezra.Application.Interfaces;

public interface IFollowUpTaskRepository
{
    Task<IReadOnlyList<FollowUpTask>> GetAllAsync(
        FollowUpTaskStatus? status = null,
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<FollowUpTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FollowUpTask> AddAsync(FollowUpTask task, CancellationToken cancellationToken = default);
    Task UpdateAsync(FollowUpTask task, CancellationToken cancellationToken = default);
}
