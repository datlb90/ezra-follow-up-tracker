using Ezra.Application.Interfaces;
using Ezra.Domain.Entities;
using Ezra.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace Ezra.Infrastructure.Repositories;

public class TaskActivityRepository : ITaskActivityRepository
{
    private readonly AppDbContext _context;

    public TaskActivityRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TaskActivity>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _context.TaskActivities
            .Where(a => a.FollowUpTaskId == taskId)
            .OrderByDescending(a => a.OccurredAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskActivity> AddAsync(TaskActivity activity, CancellationToken cancellationToken = default)
    {
        _context.TaskActivities.Add(activity);
        await _context.SaveChangesAsync(cancellationToken);
        return activity;
    }
}
