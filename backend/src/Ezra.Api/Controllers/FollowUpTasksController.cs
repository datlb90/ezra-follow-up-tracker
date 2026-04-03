using Ezra.Application.DTOs.Tasks;
using Ezra.Application.Interfaces;
using Ezra.Domain.Enums;

using Microsoft.AspNetCore.Mvc;

namespace Ezra.Api.Controllers;

[ApiController]
[Route("api/follow-up-tasks")]
public class FollowUpTasksController : ControllerBase
{
    private readonly IFollowUpTaskService _taskService;

    public FollowUpTasksController(IFollowUpTaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<FollowUpTaskResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] FollowUpTaskStatus? status,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var tasks = await _taskService.GetTasksAsync(status, search, cancellationToken);
        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FollowUpTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var task = await _taskService.GetByIdAsync(id, cancellationToken);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPost]
    [ProducesResponseType(typeof(FollowUpTaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateFollowUpTaskRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _taskService.CreateFromFindingAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(FollowUpTaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateFollowUpTaskRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _taskService.UpdateAsync(id, request, cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardSummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardSummary(CancellationToken cancellationToken)
    {
        var summary = await _taskService.GetDashboardSummaryAsync(cancellationToken);
        return Ok(summary);
    }
}
