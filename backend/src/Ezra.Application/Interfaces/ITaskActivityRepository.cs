using Ezra.Domain.Entities;

namespace Ezra.Application.Interfaces;

public interface ITaskActivityRepository
{
    Task<IReadOnlyList<TaskActivity>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<TaskActivity> AddAsync(TaskActivity activity, CancellationToken cancellationToken = default);
}
