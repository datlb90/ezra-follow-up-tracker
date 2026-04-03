using Ezra.Application.Services;
using Ezra.Domain.Entities;
using Ezra.Domain.Enums;

namespace Ezra.Application.Tests;

public class TaskPriorityServiceTests
{
    private readonly TaskPriorityService _sut = new();

    private static Finding CreateFinding(FindingSeverity? severity = null) => new()
    {
        Id = Guid.NewGuid(),
        ReportId = Guid.NewGuid(),
        Title = "Test finding",
        Description = "Test description",
        Severity = severity
    };

    private static FollowUpTask CreateTask(
        FollowUpTaskStatus status = FollowUpTaskStatus.NotStarted,
        DateTime? dueAt = null) => new()
    {
        Id = Guid.NewGuid(),
        FindingId = Guid.NewGuid(),
        Title = "Test task",
        Status = status,
        DueAt = dueAt,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    [Fact]
    public void HighSeverity_DueSoon_NotStarted_ReturnsHigh()
    {
        var finding = CreateFinding(FindingSeverity.High);
        var task = CreateTask(FollowUpTaskStatus.NotStarted, DateTime.UtcNow.AddDays(2));

        var result = _sut.Evaluate(task, finding);

        Assert.Equal(TaskPriorityLevel.High, result.PriorityLevel);
        Assert.Equal(100, result.PriorityScore);
    }

    [Fact]
    public void LowSeverity_NoDueDate_Completed_ReturnsLow()
    {
        var finding = CreateFinding(FindingSeverity.Low);
        var task = CreateTask(FollowUpTaskStatus.Completed, dueAt: null);

        var result = _sut.Evaluate(task, finding);

        Assert.Equal(TaskPriorityLevel.Low, result.PriorityLevel);
        Assert.Equal(10, result.PriorityScore);
    }

    [Fact]
    public void OverdueTask_ReturnsCritical_RegardlessOfScore()
    {
        var finding = CreateFinding(FindingSeverity.Low);
        var task = CreateTask(FollowUpTaskStatus.Completed, DateTime.UtcNow.AddDays(-1));

        var result = _sut.Evaluate(task, finding);

        Assert.Equal(TaskPriorityLevel.Critical, result.PriorityLevel);
        Assert.Contains("OVERDUE", result.PriorityReason);
    }

    [Fact]
    public void MediumSeverity_DueIn5Days_InProgress_ReturnsMedium()
    {
        var finding = CreateFinding(FindingSeverity.Medium);
        var task = CreateTask(FollowUpTaskStatus.InProgress, DateTime.UtcNow.AddDays(5));

        var result = _sut.Evaluate(task, finding);

        // 30 (medium) + 20 (4-7 days) + 5 (in progress) = 55
        Assert.Equal(TaskPriorityLevel.Medium, result.PriorityLevel);
        Assert.Equal(55, result.PriorityScore);
    }

    [Fact]
    public void NullSeverity_DefaultsToLowScore()
    {
        var finding = CreateFinding(severity: null);
        var task = CreateTask(FollowUpTaskStatus.NotStarted, dueAt: null);

        var result = _sut.Evaluate(task, finding);

        // 10 (null → Low default) + 0 (no due date) + 10 (not started) = 20
        Assert.Equal(20, result.PriorityScore);
        Assert.Equal(TaskPriorityLevel.Low, result.PriorityLevel);
    }

    [Fact]
    public void NullDueAt_ContributesZeroScore()
    {
        var finding = CreateFinding(FindingSeverity.High);
        var task = CreateTask(FollowUpTaskStatus.NotStarted, dueAt: null);

        var result = _sut.Evaluate(task, finding);

        // 50 (high) + 0 (no due date) + 10 (not started) = 60
        Assert.Equal(60, result.PriorityScore);
        Assert.Equal(TaskPriorityLevel.Medium, result.PriorityLevel);
    }

    [Fact]
    public void ReasonString_ContainsAllComponents()
    {
        var finding = CreateFinding(FindingSeverity.High);
        var task = CreateTask(FollowUpTaskStatus.NotStarted, DateTime.UtcNow.AddDays(1));

        var result = _sut.Evaluate(task, finding);

        Assert.Contains("High severity", result.PriorityReason);
        Assert.Contains("not started", result.PriorityReason);
        Assert.Contains("due in", result.PriorityReason);
    }

    [Fact]
    public void HighSeverity_DueIn10Days_Completed_ReturnsMedium()
    {
        var finding = CreateFinding(FindingSeverity.High);
        var task = CreateTask(FollowUpTaskStatus.Completed, DateTime.UtcNow.AddDays(10));

        var result = _sut.Evaluate(task, finding);

        // 50 (high) + 0 (>7 days) + 0 (completed) = 50
        Assert.Equal(50, result.PriorityScore);
        Assert.Equal(TaskPriorityLevel.Medium, result.PriorityLevel);
    }
}
