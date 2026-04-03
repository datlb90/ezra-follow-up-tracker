using Ezra.Application.DTOs.Tasks;
using Ezra.Domain.Entities;

namespace Ezra.Application.Interfaces;

public interface ITaskPriorityService
{
    TaskPriorityResult Evaluate(FollowUpTask task, Finding finding);
}
