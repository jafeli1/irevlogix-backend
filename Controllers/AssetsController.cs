using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using irevlogix_backend.Services;
using System.Security.Claims;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableCors("AllowFrontend")]
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
            return User.FindFirst("ClientId")?.Value ?? "ADMIN_CLIENT_001";
        }

        private static string EscapeCsv(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            if (input.Contains(",") || input.Contains("\"") || input.Contains("\n"))
            {
                return "\"" + input.Replace("\"", "\"\"") + "\"";
            }
            return input;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAssets(
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] string? condition = null,
            [FromQuery] bool? isDataBearing = null,
            [FromQuery] string? location = null,
            [FromQuery] string? export = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.Assets
                    .Where(a => a.ClientId == clientId)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                    query = query.Where(a => a.AssetID.Contains(search) || a.Manufacturer!.Contains(search) || a.Model!.Contains(search) || a.SerialNumber!.Contains(search));

                if (categoryId.HasValue)
                    query = query.Where(a => a.AssetCategoryId == categoryId.Value);

                if (!string.IsNullOrEmpty(condition))
                    query = query.Where(a => a.Condition == condition);

                if (isDataBearing.HasValue)
                    query = query.Where(a => a.IsDataBearing == isDataBearing.Value);

                if (!string.IsNullOrWhiteSpace(location))
                {
                    var loc = location.ToLower();
                    query = query.Where(a => (a.CurrentLocation ?? "").ToLower().Contains(loc));
                }

                if (!string.IsNullOrEmpty(export) && export.Equals("csv", StringComparison.OrdinalIgnoreCase))
                {
                    var all = await query
                        .OrderBy(a => a.AssetID)
                        .Select(a => new
                        {
                            AssetID = a.AssetID,
                            Category = a.AssetCategory != null ? a.AssetCategory.Name : "",
                            Manufacturer = a.Manufacturer ?? "",
                            Model = a.Model ?? "",
                            SerialNumber = a.SerialNumber ?? "",
                            Condition = a.Condition ?? "",
                            EstimatedValue = a.EstimatedValue ?? 0,
                            IsDataBearing = a.IsDataBearing,
                            CurrentLocation = a.CurrentLocation ?? "",
                            Status = a.CurrentStatus != null ? a.CurrentStatus.StatusName : ""
                        })
                        .ToListAsync();

                    var lines = new List<string>();
                    lines.Add("AssetID,Category,Manufacturer,Model,SerialNumber,Condition,EstimatedValue,IsDataBearing,CurrentLocation,Status");
                    foreach (var a in all)
                    {
                        string csvLine = string.Join(",", new[]
                        {
                            EscapeCsv(a.AssetID),
                            EscapeCsv(a.Category),
                            EscapeCsv(a.Manufacturer),
                            EscapeCsv(a.Model),
                            EscapeCsv(a.SerialNumber),
                            EscapeCsv(a.Condition),
                            a.EstimatedValue.ToString(),
                            a.IsDataBearing ? "Yes" : "No",
                            EscapeCsv(a.CurrentLocation),
                            EscapeCsv(a.Status)
                        });
                        lines.Add(csvLine);
                    }
                    var bytes = System.Text.Encoding.UTF8.GetBytes(string.Join("\n", lines));
                    return File(bytes, "text/csv", "assets_export.csv");
                }

                var totalCount = await query.CountAsync();
                var assets = await query
                    .OrderBy(a => a.AssetID)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new
                    {
                        id = a.Id,
                        assetID = a.AssetID,
                        assetCategory = new { name = a.AssetCategory != null ? a.AssetCategory.Name : null },
                        manufacturer = a.Manufacturer ?? "",
                        model = a.Model ?? "",
                        condition = a.Condition ?? "",
                        estimatedValue = a.EstimatedValue ?? 0,
                        isDataBearing = a.IsDataBearing,
                        currentLocation = a.CurrentLocation ?? "",
                        currentStatus = new { statusName = a.CurrentStatus != null ? a.CurrentStatus.StatusName : null },
                        dateCreated = a.DateCreated,
                        clientName = (string?)null
                    })
                    .ToListAsync();

                Response.Headers["X-Total-Count"] = totalCount.ToString();
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
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userId = 1;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId)) userId = parsedUserId;

                var existingAsset = await _context.Assets
                    .Where(a => a.AssetID == request.AssetID && a.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (existingAsset != null)
                    return BadRequest("Asset with this ID already exists");

                int? effectiveStatusId = request.CurrentStatusId;
                if (!effectiveStatusId.HasValue)
                {
                    effectiveStatusId = await _context.AssetTrackingStatuses
                        .Where(s => s.ClientId == clientId && s.IsActive && s.StatusName == "Received")
                        .Select(s => (int?)s.Id)
                        .FirstOrDefaultAsync();
                }

                var asset = new Asset
                {
                    AssetID = request.AssetID,
                    AssetCategoryId = request.AssetCategoryId,
                    Manufacturer = request.Manufacturer,
                    Model = request.Model,
                    SerialNumber = request.SerialNumber,
                    Description = request.Description ?? string.Empty,
                    Condition = request.Condition,
                    EstimatedValue = request.EstimatedValue,
                    IsDataBearing = request.IsDataBearing,
                    StorageDeviceType = request.StorageDeviceType,
                    DataSanitizationStatus = request.DataSanitizationStatus,
                    CurrentLocation = request.CurrentLocation,
                    CurrentStatusId = effectiveStatusId,
                    Notes = request.Notes,
                    RecyclingVendorId = request.RecyclingVendorId,
                    ClientId = clientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.Assets.Add(asset);
                await _context.SaveChangesAsync();

                if (effectiveStatusId.HasValue)
                {
                    var statusName = await _context.AssetTrackingStatuses
                        .Where(s => s.Id == effectiveStatusId.Value && s.ClientId == clientId)
                        .Select(s => s.StatusName)
                        .FirstOrDefaultAsync();

                    var chainOfCustody = new ChainOfCustody
                    {
                        AssetId = asset.Id,
                        Timestamp = DateTime.UtcNow,
                        Location = request.CurrentLocation,
                        UserId = userId,
                        VendorId = request.VendorId ?? 1,
                        StatusChange = string.IsNullOrWhiteSpace(statusName) ? "Asset created" : statusName,
                        Notes = "Asset created",
                        ClientId = clientId,
                        CreatedBy = userId,
                        UpdatedBy = userId
                    };

                    _context.ChainOfCustodyRecords.Add(chainOfCustody);
                    await _context.SaveChangesAsync();
                }

                return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, asset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating asset");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<Asset>> CreateAssetBulk(CreateAssetRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userId = 1;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId)) userId = parsedUserId;

                var existingAsset = await _context.Assets
                    .Where(a => a.AssetID == request.AssetID && a.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (existingAsset != null)
                    return BadRequest($"Asset with ID '{request.AssetID}' already exists");

                int? effectiveStatusId = request.CurrentStatusId;
                if (!effectiveStatusId.HasValue)
                {
                    effectiveStatusId = await _context.AssetTrackingStatuses
                        .Where(s => s.ClientId == clientId && s.IsActive && s.StatusName == "Received")
                        .Select(s => (int?)s.Id)
                        .FirstOrDefaultAsync();
                }

                var asset = new Asset
                {
                    AssetID = request.AssetID,
                    AssetCategoryId = request.AssetCategoryId,
                    Manufacturer = request.Manufacturer,
                    Model = request.Model,
                    SerialNumber = request.SerialNumber,
                    Description = request.Description ?? string.Empty,
                    Condition = request.Condition ?? "Unknown",
                    EstimatedValue = request.EstimatedValue,
                    IsDataBearing = request.IsDataBearing,
                    StorageDeviceType = request.StorageDeviceType,
                    DataSanitizationStatus = request.DataSanitizationStatus,
                    CurrentLocation = request.CurrentLocation,
                    CurrentStatusId = effectiveStatusId,
                    Notes = request.Notes,
                    ClientId = clientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.Assets.Add(asset);
                await _context.SaveChangesAsync();

                if (effectiveStatusId.HasValue)
                {
                    var statusName = await _context.AssetTrackingStatuses
                        .Where(s => s.Id == effectiveStatusId.Value && s.ClientId == clientId)
                        .Select(s => s.StatusName)
                        .FirstOrDefaultAsync();

                    var chainOfCustody = new ChainOfCustody
                    {
                        AssetId = asset.Id,
                        Timestamp = DateTime.UtcNow,
                        Location = request.CurrentLocation,
                        UserId = userId,
                        VendorId = request.VendorId ?? 1,
                        StatusChange = string.IsNullOrWhiteSpace(statusName) ? "Asset created via bulk upload" : statusName,
                        Notes = "Asset created via bulk upload",
                        ClientId = clientId,
                        CreatedBy = userId,
                        UpdatedBy = userId
                    };

                    _context.ChainOfCustodyRecords.Add(chainOfCustody);
                    await _context.SaveChangesAsync();
                }

                return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, asset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating asset via bulk upload");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsset(int id, UpdateAssetRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userId = 1;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId)) userId = parsedUserId;

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
                asset.Description = request.Description ?? asset.Description;
                asset.Condition = request.Condition ?? asset.Condition;
                asset.EstimatedValue = request.EstimatedValue ?? asset.EstimatedValue;
                asset.IsDataBearing = request.IsDataBearing ?? asset.IsDataBearing;
                asset.StorageDeviceType = request.StorageDeviceType ?? asset.StorageDeviceType;
                asset.DataSanitizationStatus = request.DataSanitizationStatus ?? asset.DataSanitizationStatus;
                asset.DataSanitizationMethod = request.DataSanitizationMethod ?? asset.DataSanitizationMethod;
                asset.DataSanitizationDate = request.DataSanitizationDate ?? asset.DataSanitizationDate;
                asset.DataDestructionCost = request.DataDestructionCost ?? asset.DataDestructionCost;
                asset.DataSanitizationCertificate = request.DataSanitizationCertificate ?? asset.DataSanitizationCertificate;
                asset.CurrentLocation = request.CurrentLocation ?? asset.CurrentLocation;
                asset.CurrentStatusId = request.CurrentStatusId ?? asset.CurrentStatusId;
                asset.Notes = request.Notes ?? asset.Notes;
                asset.RecyclingVendorId = request.RecyclingVendorId ?? asset.RecyclingVendorId;
                
                asset.SourceShipmentId = request.SourceShipmentId ?? asset.SourceShipmentId;
                asset.OriginalPurchaseDate = request.OriginalPurchaseDate ?? asset.OriginalPurchaseDate;
                asset.OriginalCost = request.OriginalCost ?? asset.OriginalCost;
                asset.ActualSalePrice = request.ActualSalePrice ?? asset.ActualSalePrice;
                asset.CostOfRecovery = request.CostOfRecovery ?? asset.CostOfRecovery;
                asset.CurrentResponsibleUserId = request.CurrentResponsibleUserId ?? asset.CurrentResponsibleUserId;
                asset.ReuseDisposition = request.ReuseDisposition ?? asset.ReuseDisposition;
                asset.ResaleDisposition = request.ResaleDisposition ?? asset.ResaleDisposition;
                asset.ReuseRecipient = request.ReuseRecipient ?? asset.ReuseRecipient;
                asset.ReusePurpose = request.ReusePurpose ?? asset.ReusePurpose;
                asset.ReuseDate = request.ReuseDate ?? asset.ReuseDate;
                asset.FairMarketValue = request.FairMarketValue ?? asset.FairMarketValue;
                asset.Buyer = request.Buyer ?? asset.Buyer;
                asset.SaleDate = request.SaleDate ?? asset.SaleDate;
                asset.ResalePlatform = request.ResalePlatform ?? asset.ResalePlatform;
                asset.CostOfSale = request.CostOfSale ?? asset.CostOfSale;
                asset.SalesInvoice = request.SalesInvoice ?? asset.SalesInvoice;
                asset.RecyclingDate = request.RecyclingDate ?? asset.RecyclingDate;
                asset.RecyclingCost = request.RecyclingCost ?? asset.RecyclingCost;
                asset.CertificateOfRecycling = request.CertificateOfRecycling ?? asset.CertificateOfRecycling;
                asset.ProcessingLotId = request.ProcessingLotId ?? asset.ProcessingLotId;
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
                        VendorId = 1, // Default vendor for chain of custody
                        StatusChange = "Asset updated",
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
                    .Include(c => c.Vendor)
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

        [HttpPost("{id}/chain-of-custody")]
        public async Task<ActionResult<ChainOfCustody>> CreateChainOfCustodyEntry(int id, CreateChainOfCustodyRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = 1;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId)) userId = parsedUserId;

                var asset = await _context.Assets
                    .Where(a => a.Id == id && a.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (asset == null)
                    return NotFound("Asset not found");

                var vendor = await _context.Vendors
                    .Where(v => v.Id == request.VendorId && v.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (vendor == null)
                    return BadRequest("Invalid vendor");

                var user = await _context.Users
                    .Where(u => u.Id == request.UserId && u.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return BadRequest("Invalid user");

                var chainOfCustody = new ChainOfCustody
                {
                    AssetId = id,
                    Timestamp = DateTime.UtcNow,
                    Location = request.Location,
                    UserId = request.UserId,
                    VendorId = request.VendorId,
                    StatusChange = request.StatusChange,
                    Notes = request.Notes,
                    ClientId = clientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.ChainOfCustodyRecords.Add(chainOfCustody);
                await _context.SaveChangesAsync();

                var createdEntry = await _context.ChainOfCustodyRecords
                    .Where(c => c.Id == chainOfCustody.Id)
                    .Include(c => c.User)
                    .Include(c => c.Vendor)
                    .FirstOrDefaultAsync();

                return Ok(createdEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating chain of custody entry for asset {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{id}/documents")]
        public async Task<ActionResult<IEnumerable<AssetDocument>>> GetAssetDocuments(int id)
        {
            try
            {
                var clientId = GetClientId();
                var asset = await _context.Assets.Where(a => a.Id == id && a.ClientId == clientId).FirstOrDefaultAsync();
                if (asset == null) return NotFound();
                var docs = await _context.AssetDocuments
                    .Where(d => d.AssetId == id && d.ClientId == clientId)
                    .OrderByDescending(d => d.DateCreated)
                    .ToListAsync();
                return Ok(docs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving asset documents for asset {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/documents")]
        public async Task<ActionResult> UploadAssetDocument(int id, IFormFile file, [FromForm] string? description = null)
        {
            try
            {
                var clientId = GetClientId();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = 1;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId)) userId = parsedUserId;

                var asset = await _context.Assets.Where(a => a.Id == id && a.ClientId == clientId).FirstOrDefaultAsync();
                if (asset == null) return NotFound();
                if (file == null || file.Length == 0) return BadRequest("No file uploaded");

                var uploadsPath = Path.Combine("upload", clientId, "Assets", id.ToString());
                Directory.CreateDirectory(uploadsPath);
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var document = new AssetDocument
                {
                    AssetId = id,
                    FileName = file.FileName,
                    FilePath = filePath,
                    ContentType = file.ContentType,
                    Description = description,
                    ClientId = clientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.AssetDocuments.Add(document);
                await _context.SaveChangesAsync();

                return Ok(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading asset document for asset {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}/documents/{documentId}")]
        public async Task<IActionResult> DeleteAssetDocument(int id, int documentId)
        {
            try
            {
                var clientId = GetClientId();
                var document = await _context.AssetDocuments
                    .Where(d => d.Id == documentId && d.AssetId == id && d.ClientId == clientId)
                    .FirstOrDefaultAsync();
                if (document == null) return NotFound();

                if (!string.IsNullOrEmpty(document.FilePath) && System.IO.File.Exists(document.FilePath))
                {
                    System.IO.File.Delete(document.FilePath);
                }

                _context.AssetDocuments.Remove(document);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting asset document {DocumentId} for asset {Id}", documentId, id);
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
        public string? Description { get; set; }
        public string? Condition { get; set; }
        public decimal? EstimatedValue { get; set; }
        public bool IsDataBearing { get; set; } = false;
        public string? StorageDeviceType { get; set; }
        public string? DataSanitizationStatus { get; set; }
        public string? CurrentLocation { get; set; }
        public int? CurrentStatusId { get; set; }
        public string? Notes { get; set; }
        public int? VendorId { get; set; }
        public int? RecyclingVendorId { get; set; }
    }

    public class UpdateAssetRequest
    {
        public int? AssetCategoryId { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? Description { get; set; }
        public string? Condition { get; set; }
        public decimal? EstimatedValue { get; set; }
        public bool? IsDataBearing { get; set; }
        public string? StorageDeviceType { get; set; }
        public string? DataSanitizationStatus { get; set; }
        public string? DataSanitizationMethod { get; set; }
        public DateTime? DataSanitizationDate { get; set; }
        public decimal? DataDestructionCost { get; set; }
        public string? DataSanitizationCertificate { get; set; }
        public string? CurrentLocation { get; set; }
        public int? CurrentStatusId { get; set; }
        public string? Notes { get; set; }
        public int? VendorId { get; set; }
        public int? RecyclingVendorId { get; set; }
        
        public int? SourceShipmentId { get; set; }
        public DateTime? OriginalPurchaseDate { get; set; }
        public decimal? OriginalCost { get; set; }
        public decimal? ActualSalePrice { get; set; }
        public decimal? CostOfRecovery { get; set; }
        public int? CurrentResponsibleUserId { get; set; }
        public bool? ReuseDisposition { get; set; }
        public bool? ResaleDisposition { get; set; }
        public string? ReuseRecipient { get; set; }
        public string? ReusePurpose { get; set; }
        public DateTime? ReuseDate { get; set; }
        public decimal? FairMarketValue { get; set; }
        public string? Buyer { get; set; }
        public DateTime? SaleDate { get; set; }
        public string? ResalePlatform { get; set; }
        public decimal? CostOfSale { get; set; }
        public string? SalesInvoice { get; set; }
        public DateTime? RecyclingDate { get; set; }
        public decimal? RecyclingCost { get; set; }
        public string? CertificateOfRecycling { get; set; }
        public int? ProcessingLotId { get; set; }
    }

    public class CategorizeAssetRequest
    {
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? Description { get; set; }
    }

    public class CreateChainOfCustodyRequest
    {
        public string? Location { get; set; }
        public int UserId { get; set; }
        public int VendorId { get; set; }
        public string StatusChange { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
