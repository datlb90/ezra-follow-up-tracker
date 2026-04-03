using Ezra.Application.DTOs.Tasks;
using Ezra.Application.Interfaces;
using Ezra.Domain.Entities;
using Ezra.Domain.Enums;

namespace Ezra.Application.Services;

public class FollowUpTaskService : IFollowUpTaskService
{
    private readonly IFollowUpTaskRepository _taskRepository;
    private readonly ITaskActivityRepository _activityRepository;
    private readonly ITaskPriorityService _priorityService;
    private readonly IReportRepository _reportRepository;

    public FollowUpTaskService(
        IFollowUpTaskRepository taskRepository,
        ITaskActivityRepository activityRepository,
        ITaskPriorityService priorityService,
        IReportRepository reportRepository)
    {
        _taskRepository = taskRepository;
        _activityRepository = activityRepository;
        _priorityService = priorityService;
        _reportRepository = reportRepository;
    }

    public async Task<IReadOnlyList<FollowUpTaskResponse>> GetTasksAsync(
        FollowUpTaskStatus? status = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var tasks = await _taskRepository.GetAllAsync(status, search, cancellationToken);

        return tasks.Select(MapToResponse).ToList();
    }

    public async Task<FollowUpTaskResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        return task is null ? null : MapToResponse(task);
    }

    public async Task<FollowUpTaskResponse?> CreateFromFindingAsync(
        CreateFollowUpTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var findingExists = await _reportRepository.FindingExistsAsync(request.FindingId, cancellationToken);
        if (!findingExists) return null;

        var now = DateTime.UtcNow;

        var task = new FollowUpTask
        {
            Id = Guid.NewGuid(),
            FindingId = request.FindingId,
            Title = request.Title,
            Description = request.Description,
            Status = FollowUpTaskStatus.NotStarted,
            DueAt = request.DueAt,
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

        var loaded = await _taskRepository.GetByIdAsync(created.Id, cancellationToken);
        return MapToResponse(loaded!);
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
        if (request.DueAt.HasValue) task.DueAt = request.DueAt.Value;
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

    private FollowUpTaskResponse MapToResponse(FollowUpTask task)
    {
        var priorityResult = _priorityService.Evaluate(task, task.Finding);

        return new FollowUpTaskResponse
        {
            Id = task.Id,
            FindingId = task.FindingId,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            DueAt = task.DueAt,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            PriorityScore = priorityResult.PriorityScore,
            PriorityLevel = priorityResult.PriorityLevel,
            PriorityReason = priorityResult.PriorityReason
        };
    }
}
