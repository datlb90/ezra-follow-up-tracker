using Ezra.Application.DTOs.Tasks;
using Ezra.Domain.Enums;

namespace Ezra.Application.Interfaces;

public interface IFollowUpTaskService
{
    Task<IReadOnlyList<FollowUpTaskResponse>> GetTasksAsync(
        FollowUpTaskStatus? status = null,
        TaskPriority? priority = null,
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<FollowUpTaskResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FollowUpTaskResponse> CreateFromFindingAsync(CreateFollowUpTaskRequest request, CancellationToken cancellationToken = default);
    Task<FollowUpTaskResponse?> UpdateAsync(Guid id, UpdateFollowUpTaskRequest request, CancellationToken cancellationToken = default);
}
