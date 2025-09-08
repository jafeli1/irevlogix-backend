using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using System.Security.Claims;
using System.Text.Json;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScheduledReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ScheduledReportsController> _logger;

        public ScheduledReportsController(ApplicationDbContext context, ILogger<ScheduledReportsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private int GetClientId()
        {
            var clientIdString = User.FindFirst("ClientId")?.Value ?? throw new UnauthorizedAccessException("ClientId not found in token");
            return int.Parse(clientIdString);
        }

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        [HttpGet]
        public async Task<IActionResult> GetScheduledReports()
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.ScheduledReports
                    .Include(sr => sr.CreatedByUser)
                    .AsQueryable();

                if (!IsAdministrator())
                {
                    query = query.Where(sr => sr.ClientId == clientId);
                }

                var reports = await query
                    .OrderByDescending(sr => sr.DateCreated)
                    .ToListAsync();

                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving scheduled reports");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduledReport(int id)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.ScheduledReports
                    .Include(sr => sr.CreatedByUser)
                    .AsQueryable();

                if (!IsAdministrator())
                {
                    query = query.Where(sr => sr.ClientId == clientId);
                }

                var report = await query.FirstOrDefaultAsync(sr => sr.Id == id);

                if (report == null)
                {
                    return NotFound();
                }

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving scheduled report {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateScheduledReport([FromBody] CreateScheduledReportRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = GetUserId();

                var scheduledReport = new ScheduledReport
                {
                    Name = request.Name,
                    DataSource = request.DataSource,
                    SelectedColumns = JsonSerializer.Serialize(request.SelectedColumns),
                    Filters = request.Filters != null ? JsonSerializer.Serialize(request.Filters) : null,
                    Sorting = request.Sorting != null ? JsonSerializer.Serialize(request.Sorting) : null,
                    Frequency = request.Frequency,
                    Recipients = JsonSerializer.Serialize(request.Recipients),
                    DeliveryTime = request.DeliveryTime,
                    DayOfWeek = request.DayOfWeek,
                    DayOfMonth = request.DayOfMonth,
                    IsActive = true,
                    ClientId = clientId,
                    CreatedByUserId = userId,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                };

                scheduledReport.NextRunDate = CalculateNextRunDate(scheduledReport);

                _context.ScheduledReports.Add(scheduledReport);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetScheduledReport), new { id = scheduledReport.Id }, scheduledReport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating scheduled report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateScheduledReport(int id, [FromBody] UpdateScheduledReportRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.ScheduledReports.AsQueryable();

                if (!IsAdministrator())
                {
                    query = query.Where(sr => sr.ClientId == clientId);
                }

                var report = await query.FirstOrDefaultAsync(sr => sr.Id == id);

                if (report == null)
                {
                    return NotFound();
                }

                report.Name = request.Name;
                report.DataSource = request.DataSource;
                report.SelectedColumns = JsonSerializer.Serialize(request.SelectedColumns);
                report.Filters = request.Filters != null ? JsonSerializer.Serialize(request.Filters) : null;
                report.Sorting = request.Sorting != null ? JsonSerializer.Serialize(request.Sorting) : null;
                report.Frequency = request.Frequency;
                report.Recipients = JsonSerializer.Serialize(request.Recipients);
                report.DeliveryTime = request.DeliveryTime;
                report.DayOfWeek = request.DayOfWeek;
                report.DayOfMonth = request.DayOfMonth;
                report.IsActive = request.IsActive;
                report.DateModified = DateTime.UtcNow;

                report.NextRunDate = CalculateNextRunDate(report);

                await _context.SaveChangesAsync();

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating scheduled report {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScheduledReport(int id)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.ScheduledReports.AsQueryable();

                if (!IsAdministrator())
                {
                    query = query.Where(sr => sr.ClientId == clientId);
                }

                var report = await query.FirstOrDefaultAsync(sr => sr.Id == id);

                if (report == null)
                {
                    return NotFound();
                }

                _context.ScheduledReports.Remove(report);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting scheduled report {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/toggle")]
        public async Task<IActionResult> ToggleScheduledReport(int id)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.ScheduledReports.AsQueryable();

                if (!IsAdministrator())
                {
                    query = query.Where(sr => sr.ClientId == clientId);
                }

                var report = await query.FirstOrDefaultAsync(sr => sr.Id == id);

                if (report == null)
                {
                    return NotFound();
                }

                report.IsActive = !report.IsActive;
                report.DateModified = DateTime.UtcNow;

                if (report.IsActive)
                {
                    report.NextRunDate = CalculateNextRunDate(report);
                }
                else
                {
                    report.NextRunDate = null;
                }

                await _context.SaveChangesAsync();

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling scheduled report {Id}", id);
                return StatusCode(500, "Internal server error");
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
            var currentMonth = baseDateTime.Year * 12 + baseDateTime.Month;
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
    }

    public class CreateScheduledReportRequest
    {
        public string Name { get; set; } = string.Empty;
        public string DataSource { get; set; } = string.Empty;
        public string[] SelectedColumns { get; set; } = Array.Empty<string>();
        public object? Filters { get; set; }
        public object? Sorting { get; set; }
        public string Frequency { get; set; } = string.Empty;
        public string[] Recipients { get; set; } = Array.Empty<string>();
        public TimeSpan DeliveryTime { get; set; } = TimeSpan.FromHours(9);
        public int? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
    }

    public class UpdateScheduledReportRequest : CreateScheduledReportRequest
    {
        public bool IsActive { get; set; } = true;
    }
}
