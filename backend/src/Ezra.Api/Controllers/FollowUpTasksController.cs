using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Ezra.Application.DTOs.Tasks;
using Ezra.Application.Interfaces;
using Ezra.Domain.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ezra.Api.Controllers;

[Authorize]
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
        return task is null ? NotFound(new { message = "Task not found." }) : Ok(task);
    }

    [HttpPost]
    [ProducesResponseType(typeof(FollowUpTaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateFollowUpTaskRequest request,
        CancellationToken cancellationToken)
    {
        var actor = TryGetCurrentUser();
        if (actor is null)
            return Unauthorized(new { message = "Invalid token." });

        var created = await _taskService.CreateFromFindingAsync(request, actor.Value.id, actor.Value.name, cancellationToken);
        if (created is null)
            return BadRequest(new { message = "Finding not found. Verify the findingId is valid." });

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
        var actor = TryGetCurrentUser();
        if (actor is null)
            return Unauthorized(new { message = "Invalid token." });

        var updated = await _taskService.UpdateAsync(id, request, actor.Value.id, actor.Value.name, cancellationToken);
        return updated is null ? NotFound(new { message = "Task not found." }) : Ok(updated);
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardSummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardSummary(CancellationToken cancellationToken)
    {
        var summary = await _taskService.GetDashboardSummaryAsync(cancellationToken);
        return Ok(summary);
    }

    private (Guid id, string name)? TryGetCurrentUser()
    {
        var raw = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (raw is null || !Guid.TryParse(raw, out var id))
            return null;

        var name = User.FindFirstValue(JwtRegisteredClaimNames.Name)
            ?? User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
        return (id, name);
    }
}
