namespace Ezra.Application.DTOs.Tasks;

public class DashboardSummaryResponse
{
    public int TotalTasks { get; init; }
    public int NotStarted { get; init; }
    public int InProgress { get; init; }
    public int Completed { get; init; }
}
