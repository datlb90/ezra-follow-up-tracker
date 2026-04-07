using Ezra.Application.DTOs.Tasks;
using Ezra.Domain.Enums;

namespace Ezra.Application.Interfaces;

public interface IFollowUpTaskService
{
    Task<IReadOnlyList<FollowUpTaskResponse>> GetTasksAsync(
        FollowUpTaskStatus? status = null,
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<FollowUpTaskResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FollowUpTaskResponse?> CreateFromFindingAsync(CreateFollowUpTaskRequest request, Guid actorId, string actorName, CancellationToken cancellationToken = default);
    Task<FollowUpTaskResponse?> UpdateAsync(Guid id, UpdateFollowUpTaskRequest request, Guid actorId, string actorName, CancellationToken cancellationToken = default);
    Task<DashboardSummaryResponse> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
}
