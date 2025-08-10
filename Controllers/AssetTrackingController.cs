using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using System.Security.Claims;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableCors("AllowFrontend")]
    public class AssetTrackingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AssetTrackingController> _logger;

        public AssetTrackingController(ApplicationDbContext context, ILogger<AssetTrackingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetClientId()
        {
            return User.FindFirst("ClientId")?.Value ?? "ADMIN_CLIENT_001";
        }

        [HttpGet("statuses")]
        public async Task<ActionResult<IEnumerable<AssetTrackingStatus>>> GetStatuses()
        {
            try
            {
                var clientId = GetClientId();
                var statuses = await _context.AssetTrackingStatuses
                    .Where(s => s.ClientId == clientId && s.IsActive)
                    .OrderBy(s => s.StatusName)
                    .ToListAsync();

                return Ok(statuses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving asset tracking statuses");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("statuses")]
        public async Task<ActionResult<AssetTrackingStatus>> CreateStatus(CreateStatusRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = 1;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId)) userId = parsedUserId;

                var existingStatus = await _context.AssetTrackingStatuses
                    .Where(s => s.StatusName == request.StatusName && s.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (existingStatus != null)
                    return BadRequest("Status with this name already exists");

                var status = new AssetTrackingStatus
                {
                    StatusName = request.StatusName,
                    Description = request.Description,
                    IsActive = request.IsActive,
                    ClientId = clientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.AssetTrackingStatuses.Add(status);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetStatuses), status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating asset tracking status");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("statuses/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = 1;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId)) userId = parsedUserId;

                var status = await _context.AssetTrackingStatuses
                    .Where(s => s.Id == id && s.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (status == null)
                    return NotFound();

                status.StatusName = request.StatusName ?? status.StatusName;
                status.Description = request.Description ?? status.Description;
                status.IsActive = request.IsActive ?? status.IsActive;
                status.UpdatedBy = userId;
                status.DateUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating asset tracking status {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{assetId}/update-status")]
        public async Task<IActionResult> UpdateAssetStatus(int assetId, UpdateAssetStatusRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = 1;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId)) userId = parsedUserId;

                var asset = await _context.Assets
                    .Where(a => a.Id == assetId && a.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (asset == null)
                    return NotFound("Asset not found");

                var status = await _context.AssetTrackingStatuses
                    .Where(s => s.Id == request.StatusId && s.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (status == null)
                    return NotFound("Status not found");

                asset.CurrentStatusId = request.StatusId;
                asset.CurrentLocation = request.Location ?? asset.CurrentLocation;
                asset.UpdatedBy = userId;
                asset.DateUpdated = DateTime.UtcNow;

                var chainOfCustody = new ChainOfCustody
                {
                    AssetId = assetId,
                    Timestamp = DateTime.UtcNow,
                    Location = request.Location,
                    UserId = userId,
                    StatusChangeId = request.StatusId,
                    Notes = request.Notes,
                    ClientId = clientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.ChainOfCustodyRecords.Add(chainOfCustody);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating asset status for asset {AssetId}", assetId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetTrackingDashboard()
        {
            try
            {
                var clientId = GetClientId();

                var totalAssets = await _context.Assets
                    .Where(a => a.ClientId == clientId)
                    .CountAsync();

                var assetsByStatus = await _context.Assets
                    .Where(a => a.ClientId == clientId)
                    .Include(a => a.CurrentStatus)
                    .GroupBy(a => a.CurrentStatus!.StatusName)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();

                var dataBearingAssets = await _context.Assets
                    .Where(a => a.ClientId == clientId && a.IsDataBearing)
                    .CountAsync();

                var recentActivity = await _context.ChainOfCustodyRecords
                    .Where(c => c.ClientId == clientId)
                    .OrderByDescending(c => c.Timestamp)
                    .Take(10)
                    .Select(c => new
                    {
                        Id = c.Id,
                        AssetId = c.AssetId,
                        Action = string.IsNullOrWhiteSpace(c.ActionType) ? c.StatusChange : c.ActionType,
                        Timestamp = c.Timestamp,
                        User = c.User != null
                            ? (c.User.FirstName + " " + c.User.LastName).Trim()
                            : c.UserId.ToString()
                    })
                    .ToListAsync();

                return Ok(new
                {
                    TotalAssets = totalAssets,
                    AssetsByStatus = assetsByStatus,
                    DataBearingAssets = dataBearingAssets,
                    RecentActivity = recentActivity
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tracking dashboard");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class CreateStatusRequest
    {
        public string StatusName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateStatusRequest
    {
        public string? StatusName { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateAssetStatusRequest
    {
        public int StatusId { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
    }
}
