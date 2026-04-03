using System.ComponentModel.DataAnnotations;

using Ezra.Domain.Enums;

namespace Ezra.Application.DTOs.Tasks;

public class UpdateFollowUpTaskRequest
{
    [StringLength(200, MinimumLength = 1)]
    public string? Title { get; init; }

    [StringLength(2000)]
    public string? Description { get; init; }
    public FollowUpTaskStatus? Status { get; init; }
    public DateTime? DueAt { get; init; }
}
