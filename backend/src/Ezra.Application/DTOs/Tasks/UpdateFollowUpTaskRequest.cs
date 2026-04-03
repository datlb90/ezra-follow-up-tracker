using System.ComponentModel.DataAnnotations;

using Ezra.Domain.Enums;

namespace Ezra.Application.DTOs.Tasks;

public class UpdateFollowUpTaskRequest
{
    [StringLength(200, MinimumLength = 1)]
    public string? Title { get; init; }

    public string? Description { get; init; }
    public FollowUpTaskStatus? Status { get; init; }
    public DateTime? DueAt { get; init; }
}
