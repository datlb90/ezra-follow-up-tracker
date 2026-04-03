using Ezra.Application.DTOs.Tasks;
using Ezra.Application.Interfaces;
using Ezra.Domain.Entities;
using Ezra.Domain.Enums;

namespace Ezra.Application.Services;

public class TaskPriorityService : ITaskPriorityService
{
    public TaskPriorityResult Evaluate(FollowUpTask task, Finding finding)
    {
        var reasons = new List<string>();

        int severityScore = GetSeverityScore(finding.Severity);
        reasons.Add(FormatSeverityReason(finding.Severity, severityScore));

        int dueDateScore = GetDueDateScore(task.DueAt);
        bool isOverdue = task.DueAt.HasValue && task.DueAt.Value.Date < DateTime.UtcNow.Date;
        if (task.DueAt.HasValue)
            reasons.Add(FormatDueDateReason(task.DueAt.Value, dueDateScore, isOverdue));

        int statusScore = GetStatusScore(task.Status);
        reasons.Add(FormatStatusReason(task.Status, statusScore));

        int totalScore = severityScore + dueDateScore + statusScore;

        if (isOverdue)
        {
            reasons.Add($"OVERDUE → Critical");
            return new TaskPriorityResult
            {
                PriorityScore = totalScore,
                PriorityLevel = TaskPriorityLevel.Critical,
                PriorityReason = string.Join(", ", reasons)
            };
        }

        var level = MapScoreToLevel(totalScore);
        reasons.Add($"= {totalScore} → {level}");

        return new TaskPriorityResult
        {
            PriorityScore = totalScore,
            PriorityLevel = level,
            PriorityReason = string.Join(", ", reasons)
        };
    }

    private static int GetSeverityScore(FindingSeverity? severity) => severity switch
    {
        FindingSeverity.High => 50,
        FindingSeverity.Medium => 30,
        FindingSeverity.Low => 10,
        _ => 10
    };

    private static int GetDueDateScore(DateTime? dueAt)
    {
        if (!dueAt.HasValue) return 0;

        int daysUntilDue = (dueAt.Value.Date - DateTime.UtcNow.Date).Days;

        return daysUntilDue switch
        {
            < 0 => 40,
            <= 3 => 40,
            <= 7 => 20,
            _ => 0
        };
    }

    private static int GetStatusScore(FollowUpTaskStatus status) => status switch
    {
        FollowUpTaskStatus.NotStarted => 10,
        FollowUpTaskStatus.InProgress => 5,
        FollowUpTaskStatus.Completed => 0,
        _ => 0
    };

    private static TaskPriorityLevel MapScoreToLevel(int score) => score switch
    {
        >= 80 => TaskPriorityLevel.High,
        >= 50 => TaskPriorityLevel.Medium,
        _ => TaskPriorityLevel.Low
    };

    private static string FormatSeverityReason(FindingSeverity? severity, int score)
    {
        string label = severity?.ToString() ?? "Unknown";
        return $"{label} severity (+{score})";
    }

    private static string FormatDueDateReason(DateTime dueAt, int score, bool isOverdue)
    {
        if (isOverdue) return $"overdue (+{score})";

        int days = (dueAt.Date - DateTime.UtcNow.Date).Days;
        return $"due in {days} day{(days == 1 ? "" : "s")} (+{score})";
    }

    private static string FormatStatusReason(FollowUpTaskStatus status, int score)
    {
        string label = status switch
        {
            FollowUpTaskStatus.NotStarted => "not started",
            FollowUpTaskStatus.InProgress => "in progress",
            FollowUpTaskStatus.Completed => "completed",
            _ => status.ToString()
        };
        return $"{label} (+{score})";
    }
}
