using Ezra.Application.DTOs.Tasks;
using Ezra.Application.Interfaces;
using Ezra.Domain.Entities;
using Ezra.Domain.Enums;

namespace Ezra.Application.Services;

public class FollowUpTaskService : IFollowUpTaskService
{
    private readonly IFollowUpTaskRepository _taskRepository;

    public FollowUpTaskService(IFollowUpTaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
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
        return MapToResponse(created);
    }

    public async Task<FollowUpTaskResponse?> UpdateAsync(
        Guid id,
        UpdateFollowUpTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, cancellationToken);
        if (task is null) return null;

        if (request.Title is not null) task.Title = request.Title;
        if (request.Description is not null) task.Description = request.Description;
        if (request.Status.HasValue) task.Status = request.Status.Value;
        if (request.Priority.HasValue) task.Priority = request.Priority.Value;
        task.UpdatedAt = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task, cancellationToken);
        return MapToResponse(task);
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
