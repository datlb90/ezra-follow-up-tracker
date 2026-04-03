using System.ComponentModel.DataAnnotations;

namespace Ezra.Application.DTOs.Tasks;

public class CreateFollowUpTaskRequest
{
    [Required]
    public Guid FindingId { get; init; }

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public DateTime? DueAt { get; init; }
}
