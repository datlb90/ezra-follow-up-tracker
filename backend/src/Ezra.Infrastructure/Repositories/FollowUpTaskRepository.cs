using Ezra.Application.Interfaces;
using Ezra.Domain.Entities;
using Ezra.Domain.Enums;
using Ezra.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace Ezra.Infrastructure.Repositories;

public class FollowUpTaskRepository : IFollowUpTaskRepository
{
    private readonly AppDbContext _context;

    public FollowUpTaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<FollowUpTask>> GetAllAsync(
        FollowUpTaskStatus? status = null,
        TaskPriority? priority = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.FollowUpTasks.AsNoTracking().AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t =>
                t.Title.Contains(search) ||
                (t.Description != null && t.Description.Contains(search)));

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<FollowUpTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FollowUpTasks
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<FollowUpTask> AddAsync(FollowUpTask task, CancellationToken cancellationToken = default)
    {
        _context.FollowUpTasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);
        return task;
    }

    public async Task UpdateAsync(FollowUpTask task, CancellationToken cancellationToken = default)
    {
        _context.FollowUpTasks.Update(task);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
