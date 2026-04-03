using Ezra.Domain.Entities;

namespace Ezra.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if (context.Reports.Any()) return;

        var reportId = Guid.NewGuid();

        var report = new Report
        {
            Id = reportId,
            Title = "Full Body MRI Screening — Sample Report",
            ReceivedAt = new DateTime(2026, 3, 15, 9, 0, 0, DateTimeKind.Utc)
        };

        var findings = new List<Finding>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ReportId = reportId,
                Title = "Thyroid nodule noted",
                Description = "A small nodule was observed on the right lobe of the thyroid. Recommend ultrasound follow-up within 6 months."
            },
            new()
            {
                Id = Guid.NewGuid(),
                ReportId = reportId,
                Title = "Mild lumbar disc degeneration",
                Description = "Early degenerative changes noted at L4-L5. Consider physical therapy evaluation if symptomatic."
            },
            new()
            {
                Id = Guid.NewGuid(),
                ReportId = reportId,
                Title = "Liver hemangioma",
                Description = "A benign hemangioma measuring 1.2 cm identified in the right hepatic lobe. No immediate action required; routine monitoring recommended."
            },
            new()
            {
                Id = Guid.NewGuid(),
                ReportId = reportId,
                Title = "Enlarged prostate (benign)",
                Description = "Mildly enlarged prostate gland. Recommend urology consultation for baseline assessment."
            }
        };

        context.Reports.Add(report);
        context.Findings.AddRange(findings);
        await context.SaveChangesAsync();
    }
}
