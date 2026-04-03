using Ezra.Application.Interfaces;
using Ezra.Application.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Ezra.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IFollowUpTaskService, FollowUpTaskService>();
        services.AddScoped<ITaskActivityService, TaskActivityService>();
        services.AddSingleton<ITaskPriorityService, TaskPriorityService>();

        return services;
    }
}
