using Ezra.Application.DTOs.Activities;
using Ezra.Application.Interfaces;

namespace Ezra.Application.Services;

public class TaskActivityService : ITaskActivityService
{
    private readonly ITaskActivityRepository _activityRepository;

    public TaskActivityService(ITaskActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<IReadOnlyList<TaskActivityResponse>> GetActivitiesForTaskAsync(
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var activities = await _activityRepository.GetByTaskIdAsync(taskId, cancellationToken);

        return activities.Select(a => new TaskActivityResponse
        {
            Id = a.Id,
            FollowUpTaskId = a.FollowUpTaskId,
            OccurredAt = a.OccurredAt,
            Type = a.Type,
            Summary = a.Summary
        }).ToList();
    }
}
