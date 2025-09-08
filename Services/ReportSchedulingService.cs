using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using System.Text.Json;

namespace irevlogix_backend.Services
{
    public class ReportSchedulingService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReportSchedulingService> _logger;
        private Timer? _timer;

        public ReportSchedulingService(IServiceProvider serviceProvider, ILogger<ReportSchedulingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Report Scheduling Service started");
            _timer = new Timer(ProcessScheduledReports, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Report Scheduling Service stopped");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void ProcessScheduledReports(object? state)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var reportService = scope.ServiceProvider.GetRequiredService<IReportGenerationService>();

                var now = DateTime.UtcNow;
                var dueReports = await context.ScheduledReports
                    .Include(sr => sr.Client)
                    .Where(sr => sr.IsActive && sr.NextRunDate <= now)
                    .ToListAsync();

                foreach (var report in dueReports)
                {
                    try
                    {
                        await ProcessSingleReport(report, emailService, reportService, context);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing scheduled report {ReportId}: {ReportName}", report.Id, report.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessScheduledReports");
            }
        }

        private async Task ProcessSingleReport(ScheduledReport report, IEmailService emailService, IReportGenerationService reportService, ApplicationDbContext context)
        {
            _logger.LogInformation("Processing scheduled report {ReportId}: {ReportName}", report.Id, report.Name);

            var selectedColumns = JsonSerializer.Deserialize<string[]>(report.SelectedColumns) ?? Array.Empty<string>();
            var filters = string.IsNullOrEmpty(report.Filters) ? null : JsonSerializer.Deserialize<object>(report.Filters);
            var sorting = string.IsNullOrEmpty(report.Sorting) ? null : JsonSerializer.Deserialize<object>(report.Sorting);
            var recipients = JsonSerializer.Deserialize<string[]>(report.Recipients) ?? Array.Empty<string>();

            var excelData = await reportService.GenerateExcelReportAsync(report.DataSource, selectedColumns, filters, sorting, report.ClientId);

            var fileName = $"{report.Name}_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx";
            var tenantName = report.Client?.CompanyName ?? "iRevLogix";
            var filtersSummary = string.IsNullOrEmpty(report.Filters) ? "" : "Custom filters applied";

            var success = await emailService.SendScheduledReportAsync(
                recipients,
                report.Name,
                report.DataSource,
                excelData,
                fileName,
                tenantName,
                DateTime.UtcNow,
                filtersSummary
            );

            if (success)
            {
                report.LastRunDate = DateTime.UtcNow;
                report.NextRunDate = CalculateNextRunDate(report);
                await context.SaveChangesAsync();
                _logger.LogInformation("Successfully processed scheduled report {ReportId}: {ReportName}", report.Id, report.Name);
            }
            else
            {
                _logger.LogError("Failed to send scheduled report {ReportId}: {ReportName}", report.Id, report.Name);
            }
        }

        private DateTime CalculateNextRunDate(ScheduledReport report)
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var deliveryDateTime = today.Add(report.DeliveryTime);

            return report.Frequency.ToLower() switch
            {
                "daily" => deliveryDateTime > now ? deliveryDateTime : deliveryDateTime.AddDays(1),
                "weekly" => CalculateNextWeeklyRun(deliveryDateTime, report.DayOfWeek ?? 1, now),
                "monthly" => CalculateNextMonthlyRun(deliveryDateTime, report.DayOfMonth ?? 1, now),
                _ => deliveryDateTime.AddDays(1)
            };
        }

        private DateTime CalculateNextWeeklyRun(DateTime baseDateTime, int dayOfWeek, DateTime now)
        {
            var daysUntilTarget = ((int)DayOfWeek.Sunday + dayOfWeek - (int)baseDateTime.DayOfWeek) % 7;
            var nextRun = baseDateTime.AddDays(daysUntilTarget);
            
            if (nextRun <= now)
            {
                nextRun = nextRun.AddDays(7);
            }
            
            return nextRun;
        }

        private DateTime CalculateNextMonthlyRun(DateTime baseDateTime, int dayOfMonth, DateTime now)
        {
            var targetDay = Math.Min(dayOfMonth, DateTime.DaysInMonth(baseDateTime.Year, baseDateTime.Month));
            var nextRun = new DateTime(baseDateTime.Year, baseDateTime.Month, targetDay, baseDateTime.Hour, baseDateTime.Minute, baseDateTime.Second);
            
            if (nextRun <= now)
            {
                var nextMonth = baseDateTime.AddMonths(1);
                targetDay = Math.Min(dayOfMonth, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
                nextRun = new DateTime(nextMonth.Year, nextMonth.Month, targetDay, baseDateTime.Hour, baseDateTime.Minute, baseDateTime.Second);
            }
            
            return nextRun;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
