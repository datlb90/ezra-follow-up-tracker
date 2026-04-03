using Ezra.Application.DTOs.Tasks;
using Ezra.Application.Interfaces;
using Ezra.Application.Services;
using Ezra.Domain.Entities;
using Ezra.Domain.Enums;

using NSubstitute;

namespace Ezra.Application.Tests;

public class FollowUpTaskServiceTests
{
    private readonly IFollowUpTaskRepository _taskRepo;
    private readonly ITaskActivityRepository _activityRepo;
    private readonly FollowUpTaskService _service;

    private static readonly Guid FindingId = Guid.NewGuid();

    public FollowUpTaskServiceTests()
    {
        _taskRepo = Substitute.For<IFollowUpTaskRepository>();
        _activityRepo = Substitute.For<ITaskActivityRepository>();
        _service = new FollowUpTaskService(_taskRepo, _activityRepo);
    }

    // -------------------------------------------------------
    // Creating a task from a finding
    // -------------------------------------------------------

    [Fact]
    public async Task CreateFromFinding_SetsStatusToNotStarted()
    {
        var request = new CreateFollowUpTaskRequest
        {
            FindingId = FindingId,
            Title = "Book thyroid ultrasound",
            Priority = TaskPriority.High
        };

        _taskRepo.AddAsync(Arg.Any<FollowUpTask>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<FollowUpTask>());

        _activityRepo.AddAsync(Arg.Any<TaskActivity>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<TaskActivity>());

        var result = await _service.CreateFromFindingAsync(request);

        Assert.Equal(FollowUpTaskStatus.NotStarted, result.Status);
    }

    [Fact]
    public async Task CreateFromFinding_MapsFieldsCorrectly()
    {
        var request = new CreateFollowUpTaskRequest
        {
            FindingId = FindingId,
            Title = "Schedule follow-up MRI",
            Description = "Within 6 months",
            Priority = TaskPriority.Medium
        };

        _taskRepo.AddAsync(Arg.Any<FollowUpTask>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<FollowUpTask>());

        _activityRepo.AddAsync(Arg.Any<TaskActivity>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<TaskActivity>());

        var result = await _service.CreateFromFindingAsync(request);

        Assert.Equal(FindingId, result.FindingId);
        Assert.Equal("Schedule follow-up MRI", result.Title);
        Assert.Equal("Within 6 months", result.Description);
        Assert.Equal(TaskPriority.Medium, result.Priority);
    }

    [Fact]
    public async Task CreateFromFinding_RecordsTaskCreatedActivity()
    {
        var request = new CreateFollowUpTaskRequest
        {
            FindingId = FindingId,
            Title = "Consult urologist",
            Priority = TaskPriority.Low
        };

        _taskRepo.AddAsync(Arg.Any<FollowUpTask>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<FollowUpTask>());

        _activityRepo.AddAsync(Arg.Any<TaskActivity>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<TaskActivity>());

        await _service.CreateFromFindingAsync(request);

        await _activityRepo.Received(1).AddAsync(
            Arg.Is<TaskActivity>(a => a.Type == ActivityType.TaskCreated),
            Arg.Any<CancellationToken>());
    }

    // -------------------------------------------------------
    // Updating task status and activity recording
    // -------------------------------------------------------

    [Fact]
    public async Task Update_ChangingStatus_RecordsStatusChangedActivity()
    {
        var task = CreateSampleTask(FollowUpTaskStatus.NotStarted);
        _taskRepo.GetByIdAsync(task.Id, Arg.Any<CancellationToken>()).Returns(task);

        _activityRepo.AddAsync(Arg.Any<TaskActivity>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<TaskActivity>());

        var request = new UpdateFollowUpTaskRequest { Status = FollowUpTaskStatus.InProgress };

        await _service.UpdateAsync(task.Id, request);

        await _activityRepo.Received(1).AddAsync(
            Arg.Is<TaskActivity>(a =>
                a.Type == ActivityType.StatusChanged &&
                a.Summary.Contains("NotStarted") &&
                a.Summary.Contains("InProgress")),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_SameStatus_DoesNotRecordActivity()
    {
        var task = CreateSampleTask(FollowUpTaskStatus.InProgress);
        _taskRepo.GetByIdAsync(task.Id, Arg.Any<CancellationToken>()).Returns(task);

        var request = new UpdateFollowUpTaskRequest { Status = FollowUpTaskStatus.InProgress };

        await _service.UpdateAsync(task.Id, request);

        await _activityRepo.DidNotReceive().AddAsync(
            Arg.Any<TaskActivity>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_ChangingPriorityOnly_DoesNotRecordStatusActivity()
    {
        var task = CreateSampleTask(FollowUpTaskStatus.NotStarted, TaskPriority.Low);
        _taskRepo.GetByIdAsync(task.Id, Arg.Any<CancellationToken>()).Returns(task);

        var request = new UpdateFollowUpTaskRequest { Priority = TaskPriority.High };

        await _service.UpdateAsync(task.Id, request);

        await _activityRepo.DidNotReceive().AddAsync(
            Arg.Any<TaskActivity>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_NonExistentTask_ReturnsNull()
    {
        _taskRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((FollowUpTask?)null);

        var result = await _service.UpdateAsync(Guid.NewGuid(), new UpdateFollowUpTaskRequest
        {
            Status = FollowUpTaskStatus.Completed
        });

        Assert.Null(result);
    }

    // -------------------------------------------------------
    // Filtering tasks by status and priority
    // -------------------------------------------------------

    [Fact]
    public async Task GetTasks_FilterByStatus_PassesFilterToRepository()
    {
        _taskRepo.GetAllAsync(
            FollowUpTaskStatus.Completed, null, null, Arg.Any<CancellationToken>())
            .Returns(new List<FollowUpTask>());

        await _service.GetTasksAsync(status: FollowUpTaskStatus.Completed);

        await _taskRepo.Received(1).GetAllAsync(
            FollowUpTaskStatus.Completed, null, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTasks_FilterByPriority_PassesFilterToRepository()
    {
        _taskRepo.GetAllAsync(
            null, TaskPriority.High, null, Arg.Any<CancellationToken>())
            .Returns(new List<FollowUpTask>());

        await _service.GetTasksAsync(priority: TaskPriority.High);

        await _taskRepo.Received(1).GetAllAsync(
            null, TaskPriority.High, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTasks_WithSearch_PassesSearchToRepository()
    {
        _taskRepo.GetAllAsync(
            null, null, "thyroid", Arg.Any<CancellationToken>())
            .Returns(new List<FollowUpTask>());

        await _service.GetTasksAsync(search: "thyroid");

        await _taskRepo.Received(1).GetAllAsync(
            null, null, "thyroid", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTasks_ReturnsCorrectCount()
    {
        var tasks = new List<FollowUpTask>
        {
            CreateSampleTask(FollowUpTaskStatus.NotStarted),
            CreateSampleTask(FollowUpTaskStatus.InProgress)
        };

        _taskRepo.GetAllAsync(null, null, null, Arg.Any<CancellationToken>())
            .Returns(tasks);

        var result = await _service.GetTasksAsync();

        Assert.Equal(2, result.Count);
    }

    // -------------------------------------------------------
    // Dashboard summary counts
    // -------------------------------------------------------

    [Fact]
    public async Task GetDashboardSummary_ReturnsCorrectCounts()
    {
        var tasks = new List<FollowUpTask>
        {
            CreateSampleTask(FollowUpTaskStatus.NotStarted),
            CreateSampleTask(FollowUpTaskStatus.NotStarted),
            CreateSampleTask(FollowUpTaskStatus.InProgress),
            CreateSampleTask(FollowUpTaskStatus.Completed)
        };

        _taskRepo.GetAllAsync(null, null, null, Arg.Any<CancellationToken>())
            .Returns(tasks);

        var summary = await _service.GetDashboardSummaryAsync();

        Assert.Equal(4, summary.TotalTasks);
        Assert.Equal(2, summary.NotStarted);
        Assert.Equal(1, summary.InProgress);
        Assert.Equal(1, summary.Completed);
    }

    [Fact]
    public async Task GetDashboardSummary_EmptyList_ReturnsAllZeros()
    {
        _taskRepo.GetAllAsync(null, null, null, Arg.Any<CancellationToken>())
            .Returns(new List<FollowUpTask>());

        var summary = await _service.GetDashboardSummaryAsync();

        Assert.Equal(0, summary.TotalTasks);
        Assert.Equal(0, summary.NotStarted);
        Assert.Equal(0, summary.InProgress);
        Assert.Equal(0, summary.Completed);
    }

    // -------------------------------------------------------
    // Helpers
    // -------------------------------------------------------

    private static FollowUpTask CreateSampleTask(
        FollowUpTaskStatus status = FollowUpTaskStatus.NotStarted,
        TaskPriority priority = TaskPriority.Medium)
    {
        return new FollowUpTask
        {
            Id = Guid.NewGuid(),
            FindingId = FindingId,
            Title = "Sample task",
            Status = status,
            Priority = priority,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
