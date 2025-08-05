using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using irevlogix_backend.Services;
using System.Security.Claims;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AIRecommendationService _aiService;
        private readonly ILogger<AssetsController> _logger;

        public AssetsController(ApplicationDbContext context, AIRecommendationService aiService, ILogger<AssetsController> logger)
        {
            _context = context;
            _aiService = aiService;
            _logger = logger;
        }

        private string GetClientId()
        {
            return User.FindFirst("ClientId")?.Value ?? throw new UnauthorizedAccessException("ClientId not found in token");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Asset>>> GetAssets(
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] string? condition = null,
            [FromQuery] bool? isDataBearing = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.Assets
                    .Where(a => a.ClientId == clientId)
                    .Include(a => a.AssetCategory)
                    .Include(a => a.CurrentStatus)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                    query = query.Where(a => a.AssetID.Contains(search) || a.Manufacturer!.Contains(search) || a.Model!.Contains(search) || a.SerialNumber!.Contains(search));

                if (categoryId.HasValue)
                    query = query.Where(a => a.AssetCategoryId == categoryId.Value);

                if (!string.IsNullOrEmpty(condition))
                    query = query.Where(a => a.Condition == condition);

                if (isDataBearing.HasValue)
                    query = query.Where(a => a.IsDataBearing == isDataBearing.Value);

                var totalCount = await query.CountAsync();
                var assets = await query
                    .OrderBy(a => a.AssetID)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(assets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assets");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Asset>> GetAsset(int id)
        {
            try
            {
                var clientId = GetClientId();
                var asset = await _context.Assets
                    .Where(a => a.Id == id && a.ClientId == clientId)
                    .Include(a => a.AssetCategory)
                    .Include(a => a.CurrentStatus)
                    .Include(a => a.ChainOfCustodyRecords)
                        .ThenInclude(c => c.User)
                    .Include(a => a.ChainOfCustodyRecords)
                        .ThenInclude(c => c.StatusChange)
                    .FirstOrDefaultAsync();

                if (asset == null)
                    return NotFound();

                return Ok(asset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving asset {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Asset>> CreateAsset(CreateAssetRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var existingAsset = await _context.Assets
                    .Where(a => a.AssetID == request.AssetID && a.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (existingAsset != null)
                    return BadRequest("Asset with this ID already exists");

                var asset = new Asset
                {
                    AssetID = request.AssetID,
                    AssetCategoryId = request.AssetCategoryId,
                    Manufacturer = request.Manufacturer,
                    Model = request.Model,
                    SerialNumber = request.SerialNumber,
                    Condition = request.Condition,
                    EstimatedValue = request.EstimatedValue,
                    IsDataBearing = request.IsDataBearing,
                    StorageDeviceType = request.StorageDeviceType,
                    DataSanitizationStatus = request.DataSanitizationStatus,
                    CurrentLocation = request.CurrentLocation,
                    CurrentStatusId = request.CurrentStatusId,
                    Notes = request.Notes,
                    ClientId = clientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.Assets.Add(asset);
                await _context.SaveChangesAsync();

                var chainOfCustody = new ChainOfCustody
                {
                    AssetId = asset.Id,
                    Timestamp = DateTime.UtcNow,
                    Location = request.CurrentLocation,
                    UserId = userId,
                    StatusChangeId = request.CurrentStatusId,
                    Notes = "Asset created",
                    ClientId = clientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.ChainOfCustodyRecords.Add(chainOfCustody);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, asset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating asset");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsset(int id, UpdateAssetRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var asset = await _context.Assets
                    .Where(a => a.Id == id && a.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (asset == null)
                    return NotFound();

                var oldLocation = asset.CurrentLocation;
                var oldStatusId = asset.CurrentStatusId;

                asset.AssetCategoryId = request.AssetCategoryId ?? asset.AssetCategoryId;
                asset.Manufacturer = request.Manufacturer ?? asset.Manufacturer;
                asset.Model = request.Model ?? asset.Model;
                asset.SerialNumber = request.SerialNumber ?? asset.SerialNumber;
                asset.Condition = request.Condition ?? asset.Condition;
                asset.EstimatedValue = request.EstimatedValue ?? asset.EstimatedValue;
                asset.IsDataBearing = request.IsDataBearing ?? asset.IsDataBearing;
                asset.StorageDeviceType = request.StorageDeviceType ?? asset.StorageDeviceType;
                asset.DataSanitizationStatus = request.DataSanitizationStatus ?? asset.DataSanitizationStatus;
                asset.CurrentLocation = request.CurrentLocation ?? asset.CurrentLocation;
                asset.CurrentStatusId = request.CurrentStatusId ?? asset.CurrentStatusId;
                asset.Notes = request.Notes ?? asset.Notes;
                asset.UpdatedBy = userId;
                asset.DateUpdated = DateTime.UtcNow;

                if (oldLocation != asset.CurrentLocation || oldStatusId != asset.CurrentStatusId)
                {
                    var chainOfCustody = new ChainOfCustody
                    {
                        AssetId = asset.Id,
                        Timestamp = DateTime.UtcNow,
                        Location = asset.CurrentLocation,
                        UserId = userId,
                        StatusChangeId = asset.CurrentStatusId,
                        Notes = request.Notes ?? "Asset updated",
                        ClientId = clientId,
                        CreatedBy = userId,
                        UpdatedBy = userId
                    };

                    _context.ChainOfCustodyRecords.Add(chainOfCustody);
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating asset {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            try
            {
                var clientId = GetClientId();
                var asset = await _context.Assets
                    .Where(a => a.Id == id && a.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (asset == null)
                    return NotFound();

                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting asset {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/chain-of-custody")]
        public async Task<ActionResult<IEnumerable<ChainOfCustody>>> GetChainOfCustody(int id)
        {
            try
            {
                var clientId = GetClientId();
                var asset = await _context.Assets
                    .Where(a => a.Id == id && a.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (asset == null)
                    return NotFound();

                var chainOfCustody = await _context.ChainOfCustodyRecords
                    .Where(c => c.AssetId == id && c.ClientId == clientId)
                    .Include(c => c.User)
                    .Include(c => c.StatusChange)
                    .OrderByDescending(c => c.Timestamp)
                    .ToListAsync();

                return Ok(chainOfCustody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving chain of custody for asset {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/disposition-recommendation")]
        public async Task<ActionResult<string>> GetDispositionRecommendation(int id)
        {
            try
            {
                var clientId = GetClientId();
                var asset = await _context.Assets
                    .Where(a => a.Id == id && a.ClientId == clientId)
                    .Include(a => a.AssetCategory)
                    .FirstOrDefaultAsync();

                if (asset == null)
                    return NotFound();

                var recommendation = await _aiService.GetOptimalDispositionRecommendationAsync(
                    asset.AssetCategory?.Name ?? "Unknown",
                    asset.Condition ?? "Unknown",
                    asset.EstimatedValue ?? 0,
                    asset.IsDataBearing
                );

                return Ok(new { recommendation });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting disposition recommendation for asset {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("categorize")]
        public async Task<ActionResult<string>> CategorizeAsset(CategorizeAssetRequest request)
        {
            try
            {
                var category = await _aiService.GetAutomatedAssetCategorizationAsync(
                    request.Manufacturer ?? "",
                    request.Model ?? "",
                    request.Description ?? ""
                );

                return Ok(new { category });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error categorizing asset");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class CreateAssetRequest
    {
        public string AssetID { get; set; } = string.Empty;
        public int? AssetCategoryId { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? Condition { get; set; }
        public decimal? EstimatedValue { get; set; }
        public bool IsDataBearing { get; set; } = false;
        public string? StorageDeviceType { get; set; }
        public string? DataSanitizationStatus { get; set; }
        public string? CurrentLocation { get; set; }
        public int? CurrentStatusId { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateAssetRequest
    {
        public int? AssetCategoryId { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? Condition { get; set; }
        public decimal? EstimatedValue { get; set; }
        public bool? IsDataBearing { get; set; }
        public string? StorageDeviceType { get; set; }
        public string? DataSanitizationStatus { get; set; }
        public string? CurrentLocation { get; set; }
        public int? CurrentStatusId { get; set; }
        public string? Notes { get; set; }
    }

    public class CategorizeAssetRequest
    {
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? Description { get; set; }
    }
}
