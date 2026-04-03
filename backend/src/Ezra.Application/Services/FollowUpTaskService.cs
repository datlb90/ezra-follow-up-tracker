using Ezra.Application.DTOs.Tasks;
using Ezra.Application.Interfaces;
using Ezra.Domain.Entities;
using Ezra.Domain.Enums;

namespace Ezra.Application.Services;

public class FollowUpTaskService : IFollowUpTaskService
{
    private readonly IFollowUpTaskRepository _taskRepository;
    private readonly ITaskActivityRepository _activityRepository;

    public FollowUpTaskService(
        IFollowUpTaskRepository taskRepository,
        ITaskActivityRepository activityRepository)
    {
        _taskRepository = taskRepository;
        _activityRepository = activityRepository;
    }

    public async Task<IReadOnlyList<FollowUpTaskResponse>> GetTasksAsync(
        FollowUpTaskStatus? status = null,
        TaskPriority? priority = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var tasks = await _taskRepository.GetAllAsync(status, priority, search, cancellationToken);

        return tasks.Select(MapToResponse).ToList();
    }

    public async Task<FollowUpTaskResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        return task is null ? null : MapToResponse(task);
    }

    public async Task<FollowUpTaskResponse> CreateFromFindingAsync(
        CreateFollowUpTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var task = new FollowUpTask
        {
            Id = Guid.NewGuid(),
            FindingId = request.FindingId,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = FollowUpTaskStatus.NotStarted,
            CreatedAt = now,
            UpdatedAt = now
        };

        var created = await _taskRepository.AddAsync(task, cancellationToken);

        await _activityRepository.AddAsync(new TaskActivity
        {
            Id = Guid.NewGuid(),
            FollowUpTaskId = created.Id,
            OccurredAt = now,
            Type = ActivityType.TaskCreated,
            Summary = $"Task \"{created.Title}\" created from finding"
        }, cancellationToken);

        return MapToResponse(created);
    }

    public async Task<FollowUpTaskResponse?> UpdateAsync(
        Guid id,
        UpdateFollowUpTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        if (task is null) return null;

        var previousStatus = task.Status;

        if (request.Title is not null) task.Title = request.Title;
        if (request.Description is not null) task.Description = request.Description;
        if (request.Status.HasValue) task.Status = request.Status.Value;
        if (request.Priority.HasValue) task.Priority = request.Priority.Value;
        task.UpdatedAt = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task, cancellationToken);

        if (request.Status.HasValue && request.Status.Value != previousStatus)
        {
            await _activityRepository.AddAsync(new TaskActivity
            {
                Id = Guid.NewGuid(),
                FollowUpTaskId = task.Id,
                OccurredAt = task.UpdatedAt,
                Type = ActivityType.StatusChanged,
                Summary = $"Status changed from {previousStatus} to {request.Status.Value}"
            }, cancellationToken);
        }

        return MapToResponse(task);
    }

    public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var allTasks = await _taskRepository.GetAllAsync(cancellationToken: cancellationToken);

        return new DashboardSummaryResponse
        {
            TotalTasks = allTasks.Count,
            NotStarted = allTasks.Count(t => t.Status == FollowUpTaskStatus.NotStarted),
            InProgress = allTasks.Count(t => t.Status == FollowUpTaskStatus.InProgress),
            Completed = allTasks.Count(t => t.Status == FollowUpTaskStatus.Completed)
        };
    }

    private static FollowUpTaskResponse MapToResponse(FollowUpTask task) => new()
    {
        Id = task.Id,
        FindingId = task.FindingId,
        Title = task.Title,
        Description = task.Description,
        Status = task.Status,
        Priority = task.Priority,
        CreatedAt = task.CreatedAt,
        UpdatedAt = task.UpdatedAt
    };
}
