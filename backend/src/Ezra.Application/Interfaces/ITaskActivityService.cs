using Ezra.Application.DTOs.Activities;

namespace Ezra.Application.Interfaces;

public interface ITaskActivityService
{
    Task<IReadOnlyList<TaskActivityResponse>> GetActivitiesForTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
}
