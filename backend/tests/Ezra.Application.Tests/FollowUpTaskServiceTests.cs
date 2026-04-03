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
    private readonly ITaskPriorityService _priorityService;
    private readonly IReportRepository _reportRepo;
    private readonly FollowUpTaskService _service;

    private static readonly Guid FindingId = Guid.NewGuid();

    public FollowUpTaskServiceTests()
    {
        _taskRepo = Substitute.For<IFollowUpTaskRepository>();
        _activityRepo = Substitute.For<ITaskActivityRepository>();
        _priorityService = new TaskPriorityService();
        _reportRepo = Substitute.For<IReportRepository>();
        _service = new FollowUpTaskService(_taskRepo, _activityRepo, _priorityService, _reportRepo);
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
            Title = "Book thyroid ultrasound"
        };

        SetupCreateMocks();

        var result = await _service.CreateFromFindingAsync(request);

        Assert.NotNull(result);
        Assert.Equal(FollowUpTaskStatus.NotStarted, result.Status);
    }

    [Fact]
    public async Task CreateFromFinding_MapsFieldsCorrectly()
    {
        var request = new CreateFollowUpTaskRequest
        {
            FindingId = FindingId,
            Title = "Schedule follow-up MRI",
            Description = "Within 6 months"
        };

        SetupCreateMocks();

        var result = await _service.CreateFromFindingAsync(request);

        Assert.NotNull(result);
        Assert.Equal(FindingId, result.FindingId);
        Assert.Equal("Schedule follow-up MRI", result.Title);
        Assert.Equal("Within 6 months", result.Description);
    }

    [Fact]
    public async Task CreateFromFinding_RecordsTaskCreatedActivity()
    {
        var request = new CreateFollowUpTaskRequest
        {
            FindingId = FindingId,
            Title = "Consult urologist"
        };

        SetupCreateMocks();

        await _service.CreateFromFindingAsync(request);

        await _activityRepo.Received(1).AddAsync(
            Arg.Is<TaskActivity>(a => a.Type == ActivityType.TaskCreated),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateFromFinding_InvalidFindingId_ReturnsNull()
    {
        _reportRepo.FindingExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var request = new CreateFollowUpTaskRequest
        {
            FindingId = Guid.NewGuid(),
            Title = "Should not be created"
        };

        var result = await _service.CreateFromFindingAsync(request);

        Assert.Null(result);
        await _taskRepo.DidNotReceive().AddAsync(
            Arg.Any<FollowUpTask>(), Arg.Any<CancellationToken>());
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
    // Filtering tasks by status and search
    // -------------------------------------------------------

    [Fact]
    public async Task GetTasks_FilterByStatus_PassesFilterToRepository()
    {
        _taskRepo.GetAllAsync(
            FollowUpTaskStatus.Completed, null, Arg.Any<CancellationToken>())
            .Returns(new List<FollowUpTask>());

        await _service.GetTasksAsync(status: FollowUpTaskStatus.Completed);

        await _taskRepo.Received(1).GetAllAsync(
            FollowUpTaskStatus.Completed, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTasks_WithSearch_PassesSearchToRepository()
    {
        _taskRepo.GetAllAsync(
            null, "thyroid", Arg.Any<CancellationToken>())
            .Returns(new List<FollowUpTask>());

        await _service.GetTasksAsync(search: "thyroid");

        await _taskRepo.Received(1).GetAllAsync(
            null, "thyroid", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTasks_ReturnsCorrectCount()
    {
        var tasks = new List<FollowUpTask>
        {
            CreateSampleTask(FollowUpTaskStatus.NotStarted),
            CreateSampleTask(FollowUpTaskStatus.InProgress)
        };

        _taskRepo.GetAllAsync(null, null, Arg.Any<CancellationToken>())
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

        _taskRepo.GetAllAsync(null, null, Arg.Any<CancellationToken>())
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
        _taskRepo.GetAllAsync(null, null, Arg.Any<CancellationToken>())
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

    private void SetupCreateMocks()
    {
        _reportRepo.FindingExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(true);

        FollowUpTask? captured = null;

        _taskRepo.AddAsync(Arg.Any<FollowUpTask>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                captured = ci.Arg<FollowUpTask>();
                return captured;
            });

        _taskRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                if (captured is null) return null;
                captured.Finding = new Finding
                {
                    Id = captured.FindingId,
                    ReportId = Guid.NewGuid(),
                    Title = "Sample finding",
                    Description = "Sample description",
                    Severity = FindingSeverity.Medium
                };
                return captured;
            });

        _activityRepo.AddAsync(Arg.Any<TaskActivity>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<TaskActivity>());
    }

    private static FollowUpTask CreateSampleTask(
        FollowUpTaskStatus status = FollowUpTaskStatus.NotStarted)
    {
        return new FollowUpTask
        {
            Id = Guid.NewGuid(),
            FindingId = FindingId,
            Title = "Sample task",
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Finding = new Finding
            {
                Id = FindingId,
                ReportId = Guid.NewGuid(),
                Title = "Sample finding",
                Description = "Sample description",
                Severity = FindingSeverity.Medium
            }
        };
    }
}
