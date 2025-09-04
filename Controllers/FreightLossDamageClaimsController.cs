using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using irevlogix_backend.Data;
using irevlogix_backend.Models;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FreightLossDamageClaimsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FreightLossDamageClaimsController> _logger;

        public FreightLossDamageClaimsController(ApplicationDbContext context, ILogger<FreightLossDamageClaimsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetClientId()
        {
            return User.FindFirst("ClientId")?.Value ?? throw new UnauthorizedAccessException("ClientId not found in token");
        }

        private bool IsAdministrator()
        {
            return User.IsInRole("Administrator");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetFreightLossDamageClaims(
            [FromQuery] DateTime? dateOfShipment = null,
            [FromQuery] DateTime? dateOfClaim = null,
            [FromQuery] string? claimantCity = null,
            [FromQuery] int? stateId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.FreightLossDamageClaims.AsQueryable();

                if (!IsAdministrator())
                {
                    query = query.Where(fldc => fldc.ClientId == clientId);
                }

                if (dateOfShipment.HasValue)
                    query = query.Where(fldc => fldc.DateOfShipment.HasValue && fldc.DateOfShipment.Value.Date == dateOfShipment.Value.Date);

                if (dateOfClaim.HasValue)
                    query = query.Where(fldc => fldc.DateOfClaim.HasValue && fldc.DateOfClaim.Value.Date == dateOfClaim.Value.Date);

                if (!string.IsNullOrEmpty(claimantCity))
                    query = query.Where(fldc => fldc.ClaimantCity != null && fldc.ClaimantCity.Contains(claimantCity));

                if (stateId.HasValue)
                    query = query.Where(fldc => fldc.StateId == stateId);

                var totalCount = await query.CountAsync();
                var claims = await query
                    .OrderByDescending(fldc => fldc.DateCreated)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(fldc => new
                    {
                        fldc.Id,
                        fldc.FreightLossDamageClaimId,
                        fldc.Description,
                        fldc.DateOfShipment,
                        fldc.DateOfClaim,
                        fldc.ClaimantCompanyName,
                        fldc.ClaimantCity,
                        fldc.StateId,
                        fldc.TotalValue,
                        fldc.DateCreated,
                        fldc.DateUpdated
                    })
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving freight loss damage claims");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetFreightLossDamageClaim(int id)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.FreightLossDamageClaims
                    .Where(fldc => fldc.Id == id);

                if (!IsAdministrator())
                {
                    query = query.Where(fldc => fldc.ClientId == clientId);
                }

                var claim = await query
                    .Select(fldc => new
                    {
                        fldc.Id,
                        fldc.FreightLossDamageClaimId,
                        fldc.Description,
                        fldc.RequestId,
                        fldc.DateOfShipment,
                        fldc.DateOfClaim,
                        fldc.ClaimantReferenceNumber,
                        fldc.ClaimantEmail,
                        fldc.ClaimantCompanyName,
                        fldc.ClaimantAddress,
                        fldc.ClaimantCity,
                        fldc.StateId,
                        fldc.PostalCode,
                        fldc.ClaimantPhone,
                        fldc.ClaimantFax,
                        fldc.ClaimantName,
                        fldc.ClaimantJobTitle,
                        fldc.DateClaimWasSigned,
                        fldc.NotificationOfLossDamageGivenTo,
                        fldc.NotificationOfLossDamageGivenAt,
                        fldc.NotificationOfLossDamageGivenByWhatMethod,
                        fldc.NotificationOfLossDamageGivenOn,
                        fldc.CommodityLostDamaged,
                        fldc.TotalWeight,
                        fldc.Quantity,
                        fldc.DamageDescription,
                        fldc.TotalValue,
                        fldc.ClaimAttachmentUpload1,
                        fldc.ClaimAttachmentUpload2,
                        fldc.ClaimAttachmentUpload3,
                        fldc.ClaimAttachmentUpload4,
                        fldc.DateCreated,
                        fldc.DateUpdated,
                        fldc.CreatedBy,
                        fldc.UpdatedBy,
                        fldc.ClientId
                    })
                    .FirstOrDefaultAsync();

                if (claim == null)
                    return NotFound();

                return Ok(claim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving freight loss damage claim {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateFreightLossDamageClaim(FreightLossDamageClaimRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var claim = new FreightLossDamageClaim
                {
                    FreightLossDamageClaimId = request.FreightLossDamageClaimId,
                    Description = request.Description,
                    RequestId = request.RequestId,
                    DateOfShipment = request.DateOfShipment,
                    DateOfClaim = request.DateOfClaim,
                    ClaimantReferenceNumber = request.ClaimantReferenceNumber,
                    ClaimantEmail = request.ClaimantEmail,
                    ClaimantCompanyName = request.ClaimantCompanyName,
                    ClaimantAddress = request.ClaimantAddress,
                    ClaimantCity = request.ClaimantCity,
                    StateId = request.StateId,
                    PostalCode = request.PostalCode,
                    ClaimantPhone = request.ClaimantPhone,
                    ClaimantFax = request.ClaimantFax,
                    ClaimantName = request.ClaimantName,
                    ClaimantJobTitle = request.ClaimantJobTitle,
                    DateClaimWasSigned = request.DateClaimWasSigned,
                    NotificationOfLossDamageGivenTo = request.NotificationOfLossDamageGivenTo,
                    NotificationOfLossDamageGivenAt = request.NotificationOfLossDamageGivenAt,
                    NotificationOfLossDamageGivenByWhatMethod = request.NotificationOfLossDamageGivenByWhatMethod,
                    NotificationOfLossDamageGivenOn = request.NotificationOfLossDamageGivenOn,
                    CommodityLostDamaged = request.CommodityLostDamaged,
                    TotalWeight = request.TotalWeight,
                    Quantity = request.Quantity,
                    DamageDescription = request.DamageDescription,
                    TotalValue = request.TotalValue,
                    ClaimAttachmentUpload1 = request.ClaimAttachmentUpload1,
                    ClaimAttachmentUpload2 = request.ClaimAttachmentUpload2,
                    ClaimAttachmentUpload3 = request.ClaimAttachmentUpload3,
                    ClaimAttachmentUpload4 = request.ClaimAttachmentUpload4,
                    ClientId = clientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.FreightLossDamageClaims.Add(claim);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetFreightLossDamageClaim), new { id = claim.Id }, new { id = claim.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating freight loss damage claim");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFreightLossDamageClaim(int id, FreightLossDamageClaimRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var claim = await _context.FreightLossDamageClaims
                    .Where(fldc => fldc.Id == id && fldc.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (claim == null)
                    return NotFound();

                claim.FreightLossDamageClaimId = request.FreightLossDamageClaimId;
                claim.Description = request.Description;
                claim.RequestId = request.RequestId;
                claim.DateOfShipment = request.DateOfShipment;
                claim.DateOfClaim = request.DateOfClaim;
                claim.ClaimantReferenceNumber = request.ClaimantReferenceNumber;
                claim.ClaimantEmail = request.ClaimantEmail;
                claim.ClaimantCompanyName = request.ClaimantCompanyName;
                claim.ClaimantAddress = request.ClaimantAddress;
                claim.ClaimantCity = request.ClaimantCity;
                claim.StateId = request.StateId;
                claim.PostalCode = request.PostalCode;
                claim.ClaimantPhone = request.ClaimantPhone;
                claim.ClaimantFax = request.ClaimantFax;
                claim.ClaimantName = request.ClaimantName;
                claim.ClaimantJobTitle = request.ClaimantJobTitle;
                claim.DateClaimWasSigned = request.DateClaimWasSigned;
                claim.NotificationOfLossDamageGivenTo = request.NotificationOfLossDamageGivenTo;
                claim.NotificationOfLossDamageGivenAt = request.NotificationOfLossDamageGivenAt;
                claim.NotificationOfLossDamageGivenByWhatMethod = request.NotificationOfLossDamageGivenByWhatMethod;
                claim.NotificationOfLossDamageGivenOn = request.NotificationOfLossDamageGivenOn;
                claim.CommodityLostDamaged = request.CommodityLostDamaged;
                claim.TotalWeight = request.TotalWeight;
                claim.Quantity = request.Quantity;
                claim.DamageDescription = request.DamageDescription;
                claim.TotalValue = request.TotalValue;
                claim.ClaimAttachmentUpload1 = request.ClaimAttachmentUpload1;
                claim.ClaimAttachmentUpload2 = request.ClaimAttachmentUpload2;
                claim.ClaimAttachmentUpload3 = request.ClaimAttachmentUpload3;
                claim.ClaimAttachmentUpload4 = request.ClaimAttachmentUpload4;
                claim.UpdatedBy = userId;
                claim.DateUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating freight loss damage claim {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFreightLossDamageClaim(int id)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.FreightLossDamageClaims
                    .Where(fldc => fldc.Id == id);

                if (!IsAdministrator())
                {
                    query = query.Where(fldc => fldc.ClientId == clientId);
                }

                var claim = await query.FirstOrDefaultAsync();

                if (claim == null)
                    return NotFound();

                _context.FreightLossDamageClaims.Remove(claim);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting freight loss damage claim {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/upload")]
        public async Task<ActionResult<object>> UploadDocument(int id, IFormFile file, [FromForm] string documentType, [FromForm] string? description = null)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var claim = await _context.FreightLossDamageClaims
                    .Where(fldc => fldc.Id == id && fldc.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (claim == null)
                    return NotFound();

                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                var uploadsPath = Path.Combine("upload", clientId, "FreightLossDamageClaims");
                Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                switch (documentType.ToLower())
                {
                    case "attachment1":
                        claim.ClaimAttachmentUpload1 = fileName;
                        break;
                    case "attachment2":
                        claim.ClaimAttachmentUpload2 = fileName;
                        break;
                    case "attachment3":
                        claim.ClaimAttachmentUpload3 = fileName;
                        break;
                    case "attachment4":
                        claim.ClaimAttachmentUpload4 = fileName;
                        break;
                    default:
                        return BadRequest("Invalid document type");
                }

                claim.UpdatedBy = userId;
                claim.DateUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { id = claim.Id, fileName = file.FileName, filePath = fileName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document for freight loss damage claim {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/files")]
        public async Task<ActionResult<IEnumerable<object>>> GetUploadedFiles(int id)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.FreightLossDamageClaims
                    .Where(c => c.Id == id);

                if (!IsAdministrator())
                {
                    query = query.Where(c => c.ClientId == clientId);
                }

                var claim = await query.FirstOrDefaultAsync();

                if (claim == null)
                    return NotFound();

                var uploadsPath = Path.Combine("upload", clientId, "FreightLossDamageClaims");
                
                if (!Directory.Exists(uploadsPath))
                    return Ok(new List<object>());

                var files = Directory.GetFiles(uploadsPath)
                    .Select(filePath => 
                    {
                        var fileName = Path.GetFileName(filePath);
                        var originalFileName = fileName.Contains('_') ? fileName.Substring(fileName.IndexOf('_') + 1) : fileName;
                        var fileInfo = new FileInfo(filePath);
                        var relativePath = Path.Combine("upload", clientId, "FreightLossDamageClaims", fileName).Replace("\\", "/");
                        
                        return new
                        {
                            fileName = originalFileName,
                            fullFileName = fileName,
                            filePath = "/" + relativePath,
                            fileSize = fileInfo.Length,
                            uploadDate = fileInfo.CreationTime,
                            documentType = "freight_claim_attachment"
                        };
                    })
                    .OrderByDescending(f => f.uploadDate)
                    .ToList();

                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving uploaded files for freight claim {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class FreightLossDamageClaimRequest
    {
        public int FreightLossDamageClaimId { get; set; }
        public string? Description { get; set; }
        public int? RequestId { get; set; }
        public DateTime? DateOfShipment { get; set; }
        public DateTime? DateOfClaim { get; set; }
        public string? ClaimantReferenceNumber { get; set; }
        public string? ClaimantEmail { get; set; }
        public string? ClaimantCompanyName { get; set; }
        public string? ClaimantAddress { get; set; }
        public string? ClaimantCity { get; set; }
        public int? StateId { get; set; }
        public string? PostalCode { get; set; }
        public string? ClaimantPhone { get; set; }
        public string? ClaimantFax { get; set; }
        public string? ClaimantName { get; set; }
        public string? ClaimantJobTitle { get; set; }
        public DateTime? DateClaimWasSigned { get; set; }
        public string? NotificationOfLossDamageGivenTo { get; set; }
        public string? NotificationOfLossDamageGivenAt { get; set; }
        public string? NotificationOfLossDamageGivenByWhatMethod { get; set; }
        public DateTime? NotificationOfLossDamageGivenOn { get; set; }
        public string? CommodityLostDamaged { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? Quantity { get; set; }
        public string? DamageDescription { get; set; }
        public decimal? TotalValue { get; set; }
        public string? ClaimAttachmentUpload1 { get; set; }
        public string? ClaimAttachmentUpload2 { get; set; }
        public string? ClaimAttachmentUpload3 { get; set; }
        public string? ClaimAttachmentUpload4 { get; set; }
    }
}
