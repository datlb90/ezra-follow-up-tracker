using Ezra.Domain.Enums;

namespace Ezra.Application.DTOs.Tasks;

public class TaskPriorityResult
{
    public int PriorityScore { get; init; }
    public TaskPriorityLevel PriorityLevel { get; init; }
    public string PriorityReason { get; init; } = string.Empty;
}
