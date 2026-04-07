using Ezra.Application.DTOs.Activities;
using Ezra.Application.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ezra.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/follow-up-tasks/{taskId:guid}/activities")]
public class TaskActivitiesController : ControllerBase
{
    private readonly ITaskActivityService _activityService;

    public TaskActivitiesController(ITaskActivityService activityService)
    {
        _activityService = activityService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TaskActivityResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTaskId(Guid taskId, CancellationToken cancellationToken)
    {
        var activities = await _activityService.GetActivitiesForTaskAsync(taskId, cancellationToken);
        return Ok(activities);
    }
}
