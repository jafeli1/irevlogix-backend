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

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }

        [HttpGet("statuses")]
        public async Task<ActionResult<IEnumerable<AssetTrackingStatus>>> GetStatuses(
            [FromQuery] string? statusName = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.AssetTrackingStatuses
                    .Where(s => s.IsActive);

                if (!IsAdministrator())
                {
                    query = query.Where(s => s.ClientId == clientId);
                }

                if (!string.IsNullOrEmpty(statusName))
                {
                    query = query.Where(s => s.StatusName.Contains(statusName));
                }

                var totalCount = await query.CountAsync();
                
                var statuses = await query
                    .OrderBy(s => s.SortOrder)
                    .ThenBy(s => s.StatusName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving asset tracking statuses");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("statuses/{id}")]
        public async Task<ActionResult<AssetTrackingStatus>> GetStatus(int id)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.AssetTrackingStatuses
                    .Where(s => s.Id == id);

                if (!IsAdministrator())
                {
                    query = query.Where(s => s.ClientId == clientId);
                }

                var status = await query.FirstOrDefaultAsync();

                if (status == null)
                    return NotFound();

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving asset tracking status {Id}", id);
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

                var query = _context.AssetTrackingStatuses
                    .Where(s => s.StatusName == request.StatusName);

                if (!IsAdministrator())
                {
                    query = query.Where(s => s.ClientId == clientId);
                }

                var existingStatus = await query.FirstOrDefaultAsync();

                if (existingStatus != null)
                    return BadRequest("Status with this name already exists");

                var status = new AssetTrackingStatus
                {
                    StatusName = request.StatusName,
                    Description = request.Description,
                    IsActive = request.IsActive,
                    SortOrder = request.SortOrder,
                    ColorCode = request.ColorCode,
                    ClientId = clientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.AssetTrackingStatuses.Add(status);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetStatus), new { id = status.Id }, status);
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

                var query = _context.AssetTrackingStatuses
                    .Where(s => s.Id == id);

                if (!IsAdministrator())
                {
                    query = query.Where(s => s.ClientId == clientId);
                }

                var status = await query.FirstOrDefaultAsync();

                if (status == null)
                    return NotFound();

                status.StatusName = request.StatusName ?? status.StatusName;
                status.Description = request.Description ?? status.Description;
                status.IsActive = request.IsActive ?? status.IsActive;
                status.SortOrder = request.SortOrder ?? status.SortOrder;
                status.ColorCode = request.ColorCode ?? status.ColorCode;
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

                var assetQuery = _context.Assets
                    .Where(a => a.Id == assetId);

                if (!IsAdministrator())
                {
                    assetQuery = assetQuery.Where(a => a.ClientId == clientId);
                }

                var asset = await assetQuery.FirstOrDefaultAsync();

                if (asset == null)
                    return NotFound("Asset not found");

                var statusQuery = _context.AssetTrackingStatuses
                    .Where(s => s.Id == request.StatusId);

                if (!IsAdministrator())
                {
                    statusQuery = statusQuery.Where(s => s.ClientId == clientId);
                }

                var status = await statusQuery.FirstOrDefaultAsync();

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
                    VendorId = 1, // Default vendor
                    StatusChange = status.StatusName,
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

                var assetsQuery = _context.Assets.AsQueryable();
                var chainQuery = _context.ChainOfCustodyRecords.AsQueryable();

                if (!IsAdministrator())
                {
                    assetsQuery = assetsQuery.Where(a => a.ClientId == clientId);
                    chainQuery = chainQuery.Where(c => c.ClientId == clientId);
                }

                var totalAssets = await assetsQuery.CountAsync();

                var assetsByStatus = await assetsQuery
                    .Include(a => a.CurrentStatus)
                    .Where(a => a.CurrentStatus != null)
                    .GroupBy(a => a.CurrentStatus.StatusName)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();

                var dataBearingAssets = await assetsQuery
                    .Where(a => a.IsDataBearing)
                    .CountAsync();

                var recentActivity = await chainQuery
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

        [HttpDelete("statuses/{id}")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.AssetTrackingStatuses
                    .Where(s => s.Id == id);

                if (!IsAdministrator())
                {
                    query = query.Where(s => s.ClientId == clientId);
                }

                var status = await query.FirstOrDefaultAsync();

                if (status == null)
                    return NotFound();

                _context.AssetTrackingStatuses.Remove(status);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting asset tracking status {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        private bool AssetTrackingStatusExists(int id, string clientId)
        {
            return _context.AssetTrackingStatuses.Any(s => s.Id == id && s.ClientId == clientId);
        }
    }

    public class CreateStatusRequest
    {
        public string StatusName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public string? ColorCode { get; set; }
    }

    public class UpdateStatusRequest
    {
        public string? StatusName { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public int? SortOrder { get; set; }
        public string? ColorCode { get; set; }
    }

    public class UpdateAssetStatusRequest
    {
        public int StatusId { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
    }
}
