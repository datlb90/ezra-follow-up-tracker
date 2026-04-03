using Ezra.Application.Interfaces;
using Ezra.Infrastructure.Data;
using Ezra.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ezra.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IFollowUpTaskRepository, FollowUpTaskRepository>();
        services.AddScoped<ITaskActivityRepository, TaskActivityRepository>();

        return services;
    }
}
