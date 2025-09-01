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
    public class ProcessingLotsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AIRecommendationService _aiService;
        private readonly ILogger<ProcessingLotsController> _logger;

        public ProcessingLotsController(ApplicationDbContext context, AIRecommendationService aiService, ILogger<ProcessingLotsController> logger)
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
        public async Task<ActionResult<IEnumerable<ProcessingLot>>> GetProcessingLots(
            [FromQuery] string? status = null,
            [FromQuery] string? lotId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.ProcessingLots
                    .Where(pl => pl.ClientId == clientId)
                    .Include(pl => pl.Operator)
                    .Include(pl => pl.ProcessingSteps)
                    .Include(pl => pl.ProcessedMaterials)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(pl => pl.Status == status);

                if (!string.IsNullOrEmpty(lotId))
                    query = query.Where(pl => pl.LotNumber.Contains(lotId));

                if (startDate.HasValue)
                    query = query.Where(pl => pl.StartDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(pl => pl.StartDate <= endDate.Value);

                var totalCount = await query.CountAsync();
                var lots = await query
                    .OrderByDescending(pl => pl.DateCreated)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(lots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving processing lots");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProcessingLot>> GetProcessingLot(int id)
        {
            try
            {
                var clientId = GetClientId();
                var lot = await _context.ProcessingLots
                    .Where(pl => pl.Id == id && pl.ClientId == clientId)
                    .Include(pl => pl.Operator)
                    .Include(pl => pl.ProcessingSteps.OrderBy(ps => ps.StepOrder))
                        .ThenInclude(ps => ps.ResponsibleUser)
                    .Include(pl => pl.ProcessedMaterials)
                        .ThenInclude(pm => pm.MaterialType)
                    .Include(pl => pl.IncomingShipmentItems)
                        .ThenInclude(si => si.Shipment)
                    .FirstOrDefaultAsync();

                if (lot == null)
                    return NotFound();

                return Ok(lot);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving processing lot {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProcessingLot>> CreateProcessingLot(CreateProcessingLotRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = 1;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsed)) userId = parsed;

                var lot = new ProcessingLot
                {
                    LotNumber = request.LotId ?? GenerateLotId(),
                    Description = request.Description,
                    OperatorUserId = request.OperatorUserId,
                    ProcessingCost = request.ProcessingCost,
                    StartDate = request.StartDate,
                    CompletionDate = request.CompletionDate,
                    TotalIncomingWeight = request.TotalIncomingWeight,
                    TotalProcessedWeight = request.TotalProcessedWeight,
                    IncomingMaterialCost = request.IncomingMaterialCost,
                    ExpectedRevenue = request.ExpectedRevenue,
                    ActualRevenue = request.ActualRevenue,
                    IncomingMaterialNotes = request.IncomingMaterialNotes,
                    ContaminationPercentage = request.ContaminationPercentage,
                    QualityControlNotes = request.QualityControlNotes,
                    CertificationStatus = request.CertificationStatus,
                    CertificationNumber = request.CertificationNumber,
                    ProcessingNotes = request.ProcessingNotes,
                    ProcessingMethod = request.ProcessingMethod,
                    SourceShipmentId = request.SourceShipmentId,
                    Status = "Created",
                    ClientId = clientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.ProcessingLots.Add(lot);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProcessingLot), new { id = lot.Id }, lot);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating processing lot");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProcessingLot(int id, UpdateProcessingLotRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = 1;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsed)) userId = parsed;

                var lot = await _context.ProcessingLots
                    .Where(pl => pl.Id == id && pl.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (lot == null)
                    return NotFound();

                lot.Description = request.Description ?? lot.Description;
                lot.Status = request.Status ?? lot.Status;
                lot.OperatorUserId = request.OperatorUserId ?? lot.OperatorUserId;
                lot.ProcessingCost = request.ProcessingCost ?? lot.ProcessingCost;
                lot.StartDate = request.StartDate ?? lot.StartDate;
                lot.CompletionDate = request.CompletionDate ?? lot.CompletionDate;
                lot.TotalIncomingWeight = request.TotalIncomingWeight ?? lot.TotalIncomingWeight;
                lot.TotalProcessedWeight = request.TotalProcessedWeight ?? lot.TotalProcessedWeight;
                lot.IncomingMaterialCost = request.IncomingMaterialCost ?? lot.IncomingMaterialCost;
                lot.ExpectedRevenue = request.ExpectedRevenue ?? lot.ExpectedRevenue;
                lot.ActualRevenue = request.ActualRevenue ?? lot.ActualRevenue;
                lot.IncomingMaterialNotes = request.IncomingMaterialNotes ?? lot.IncomingMaterialNotes;
                lot.ContaminationPercentage = request.ContaminationPercentage ?? lot.ContaminationPercentage;
                lot.QualityControlNotes = request.QualityControlNotes ?? lot.QualityControlNotes;
                lot.CertificationStatus = request.CertificationStatus ?? lot.CertificationStatus;
                lot.CertificationNumber = request.CertificationNumber ?? lot.CertificationNumber;
                lot.ProcessingNotes = request.ProcessingNotes ?? lot.ProcessingNotes;
                lot.ProcessingMethod = request.ProcessingMethod ?? lot.ProcessingMethod;
                lot.SourceShipmentId = request.SourceShipmentId ?? lot.SourceShipmentId;
                lot.UpdatedBy = userId;
                lot.DateUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating processing lot {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/quality-prediction")]
        public async Task<ActionResult> GetQualityPrediction(int id)
        {
            try
            {
                var clientId = GetClientId();
                var lot = await _context.ProcessingLots
                    .Where(pl => pl.Id == id && pl.ClientId == clientId)
                    .Include(pl => pl.ProcessingSteps)
                    .FirstOrDefaultAsync();

                if (lot == null)
                    return NotFound();

                var prediction = await _aiService.GetQualityGradePredictionAsync(lot);
                return Ok(prediction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quality prediction for lot {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/files")]
        public async Task<ActionResult<IEnumerable<object>>> GetUploadedFiles(int id)
        {
            try
            {
                var clientId = GetClientId();
                var lot = await _context.ProcessingLots
                    .Where(pl => pl.Id == id && pl.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (lot == null)
                    return NotFound();

                var uploadsPath = Path.Combine("upload", clientId, "ProcessingLots", id.ToString());
                
                if (!Directory.Exists(uploadsPath))
                    return Ok(new List<object>());

                var files = Directory.GetFiles(uploadsPath)
                    .Select(filePath => 
                    {
                        var fileName = Path.GetFileName(filePath);
                        var originalFileName = fileName.Contains('_') ? fileName.Substring(fileName.IndexOf('_') + 1) : fileName;
                        var fileInfo = new FileInfo(filePath);
                        var relativePath = Path.Combine("upload", clientId, "ProcessingLots", id.ToString(), fileName).Replace("\\", "/");
                        
                        return new
                        {
                            fileName = originalFileName,
                            fullFileName = fileName,
                            filePath = "/" + relativePath,
                            fileSize = fileInfo.Length,
                            uploadDate = fileInfo.CreationTime,
                            documentType = "certificate"
                        };
                    })
                    .OrderByDescending(f => f.uploadDate)
                    .ToList();

                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving uploaded files for processing lot {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/upload")]
        public async Task<ActionResult<object>> UploadCertificate(int id, IFormFile file, [FromForm] string documentType = "certificate", [FromForm] string? description = null)
        {
            try
            {
                var clientId = GetClientId();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = 1;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsed)) userId = parsed;

                var lot = await _context.ProcessingLots
                    .Where(pl => pl.Id == id && pl.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (lot == null)
                    return NotFound();

                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".txt" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Invalid file type. Allowed types: PDF, DOC, DOCX, JPG, JPEG, PNG, TXT");

                var uploadsPath = Path.Combine("upload", clientId, "ProcessingLots", id.ToString());
                Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = Path.Combine("upload", clientId, "ProcessingLots", id.ToString(), fileName);

                if (documentType.ToLower() == "certificate")
                {
                    lot.CertificationStatus = "Uploaded";
                    lot.UpdatedBy = userId;
                    lot.DateUpdated = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { id = lot.Id, fileName = file.FileName, filePath = relativePath, documentType });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading certificate for processing lot {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        private string GenerateLotId()
        {
            return $"LOT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
    }

    public class CreateProcessingLotRequest
    {
        public string? LotId { get; set; }
        public string? Description { get; set; }
        public int? OperatorUserId { get; set; }
        public decimal? ProcessingCost { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal? TotalIncomingWeight { get; set; }
        public decimal? TotalProcessedWeight { get; set; }
        public decimal? IncomingMaterialCost { get; set; }
        public decimal? ExpectedRevenue { get; set; }
        public decimal? ActualRevenue { get; set; }
        public string? IncomingMaterialNotes { get; set; }
        public decimal? ContaminationPercentage { get; set; }
        public string? QualityControlNotes { get; set; }
        public string? CertificationStatus { get; set; }
        public string? CertificationNumber { get; set; }
        public string? ProcessingNotes { get; set; }
        public string? ProcessingMethod { get; set; }
        public int? SourceShipmentId { get; set; }
    }

    public class UpdateProcessingLotRequest
    {
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int? OperatorUserId { get; set; }
        public decimal? ProcessingCost { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal? TotalIncomingWeight { get; set; }
        public decimal? TotalProcessedWeight { get; set; }
        public decimal? IncomingMaterialCost { get; set; }
        public decimal? ExpectedRevenue { get; set; }
        public decimal? ActualRevenue { get; set; }
        public string? IncomingMaterialNotes { get; set; }
        public decimal? ContaminationPercentage { get; set; }
        public string? QualityControlNotes { get; set; }
        public string? CertificationStatus { get; set; }
        public string? CertificationNumber { get; set; }
        public string? ProcessingNotes { get; set; }
        public string? ProcessingMethod { get; set; }
        public int? SourceShipmentId { get; set; }
    }
}
