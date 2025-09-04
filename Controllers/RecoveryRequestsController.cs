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
    public class RecoveryRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RecoveryRequestsController> _logger;

        public RecoveryRequestsController(ApplicationDbContext context, ILogger<RecoveryRequestsController> logger)
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
        public async Task<ActionResult<IEnumerable<object>>> GetRecoveryRequests(
            [FromQuery] string? primaryContactFirstName = null,
            [FromQuery] string? primaryContactLastName = null,
            [FromQuery] string? city = null,
            [FromQuery] int? stateId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.RecoveryRequests.AsQueryable();

                if (!IsAdministrator())
                {
                    query = query.Where(rr => rr.ClientId == clientId);
                }

                if (!string.IsNullOrEmpty(primaryContactFirstName))
                    query = query.Where(rr => rr.PrimaryContactFirstName != null && rr.PrimaryContactFirstName.Contains(primaryContactFirstName));

                if (!string.IsNullOrEmpty(primaryContactLastName))
                    query = query.Where(rr => rr.PrimaryContactLastName != null && rr.PrimaryContactLastName.Contains(primaryContactLastName));

                if (!string.IsNullOrEmpty(city))
                    query = query.Where(rr => rr.City != null && rr.City.Contains(city));

                if (stateId.HasValue)
                    query = query.Where(rr => rr.StateId == stateId);

                var totalCount = await query.CountAsync();
                var recoveryRequests = await query
                    .OrderByDescending(rr => rr.DateCreated)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(rr => new
                    {
                        rr.Id,
                        CompanyName = _context.Clients.Where(c => c.ClientId == rr.ClientId).Select(c => c.CompanyName).FirstOrDefault(),
                        rr.Description,
                        rr.PrimaryContactFirstName,
                        rr.PrimaryContactLastName,
                        rr.LocationName,
                        rr.City,
                        rr.StateId,
                        rr.DateCreated,
                        rr.DateUpdated
                    })
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(recoveryRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recovery requests");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetRecoveryRequest(int id)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.RecoveryRequests
                    .Where(rr => rr.Id == id);

                if (!IsAdministrator())
                {
                    query = query.Where(rr => rr.ClientId == clientId);
                }

                var recoveryRequest = await query.FirstOrDefaultAsync();

                if (recoveryRequest == null)
                    return NotFound();

                return Ok(recoveryRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recovery request {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateRecoveryRequest(RecoveryRequestRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var recoveryRequest = new RecoveryRequest
                {
                    ClientId = clientId,
                    RecoveryJobRequestId = await GenerateRecoveryJobRequestId(),
                    RequestAcknowledged = request.RequestAcknowledged,
                    Description = request.Description,
                    LocationName = request.LocationName,
                    Address = request.Address,
                    AddressExt = request.AddressExt,
                    City = request.City,
                    StateId = request.StateId,
                    PostalCode = request.PostalCode,
                    CountryId = request.CountryId,
                    PrimaryContactFirstName = request.PrimaryContactFirstName,
                    PrimaryContactLastName = request.PrimaryContactLastName,
                    SecondaryContactFirstName = request.SecondaryContactFirstName,
                    SecondaryContactLastName = request.SecondaryContactLastName,
                    PrimaryContactCellPhoneNumber = request.PrimaryContactCellPhoneNumber,
                    SecondaryContactCellPhoneNumber = request.SecondaryContactCellPhoneNumber,
                    PrimaryContactEmailAddress = request.PrimaryContactEmailAddress,
                    SecondaryContactEmailAddress = request.SecondaryContactEmailAddress,
                    RequestedPickUpDate = request.RequestedPickUpDate,
                    RequestedPickUpTime = request.RequestedPickUpTime,
                    CanSiteAccommodate53FeetTruck = request.CanSiteAccommodate53FeetTruck,
                    DoesSiteHaveADock = request.DoesSiteHaveADock,
                    CanHardwareBeResold = request.CanHardwareBeResold,
                    CanHardDrivesBeResold = request.CanHardDrivesBeResold,
                    IsEquipmentLoose = request.IsEquipmentLoose,
                    IsEquipmentPalletized = request.IsEquipmentPalletized,
                    IsLiftGateRequired = request.IsLiftGateRequired,
                    IsOnsiteDataDestructingRequiredWipingShredding = request.IsOnsiteDataDestructingRequiredWipingShredding,
                    TypeOfMediaToBeDestroyed = request.TypeOfMediaToBeDestroyed,
                    OtherInstructions = request.OtherInstructions,
                    InstructionUpload = request.InstructionUpload,
                    EquipmentListUpload = request.EquipmentListUpload,
                    AssetsPhotoUpload1 = request.AssetsPhotoUpload1,
                    AssetsPhotoUpload2 = request.AssetsPhotoUpload2,
                    AssetsPhotoUpload3 = request.AssetsPhotoUpload3,
                    AssetsPhotoUpload4 = request.AssetsPhotoUpload4,
                    AssetsPhotoUpload5 = request.AssetsPhotoUpload5,
                    HelpfulPhotoUpload1 = request.HelpfulPhotoUpload1,
                    HelpfulPhotoUpload2 = request.HelpfulPhotoUpload2,
                    HelpfulPhotoUpload3 = request.HelpfulPhotoUpload3,
                    IsActive = request.IsActive,
                    DateClosed = request.DateClosed,
                    ClosureComments = request.ClosureComments,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.RecoveryRequests.Add(recoveryRequest);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetRecoveryRequest), new { id = recoveryRequest.Id }, new { id = recoveryRequest.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating recovery request");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecoveryRequest(int id, RecoveryRequestRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var query = _context.RecoveryRequests
                    .Where(rr => rr.Id == id);

                if (!IsAdministrator())
                {
                    query = query.Where(rr => rr.ClientId == clientId);
                }

                var recoveryRequest = await query.FirstOrDefaultAsync();

                if (recoveryRequest == null)
                    return NotFound();

                recoveryRequest.RequestAcknowledged = request.RequestAcknowledged;
                recoveryRequest.Description = request.Description;
                recoveryRequest.LocationName = request.LocationName;
                recoveryRequest.Address = request.Address;
                recoveryRequest.AddressExt = request.AddressExt;
                recoveryRequest.City = request.City;
                recoveryRequest.StateId = request.StateId;
                recoveryRequest.PostalCode = request.PostalCode;
                recoveryRequest.CountryId = request.CountryId;
                recoveryRequest.PrimaryContactFirstName = request.PrimaryContactFirstName;
                recoveryRequest.PrimaryContactLastName = request.PrimaryContactLastName;
                recoveryRequest.SecondaryContactFirstName = request.SecondaryContactFirstName;
                recoveryRequest.SecondaryContactLastName = request.SecondaryContactLastName;
                recoveryRequest.PrimaryContactCellPhoneNumber = request.PrimaryContactCellPhoneNumber;
                recoveryRequest.SecondaryContactCellPhoneNumber = request.SecondaryContactCellPhoneNumber;
                recoveryRequest.PrimaryContactEmailAddress = request.PrimaryContactEmailAddress;
                recoveryRequest.SecondaryContactEmailAddress = request.SecondaryContactEmailAddress;
                recoveryRequest.RequestedPickUpDate = request.RequestedPickUpDate;
                recoveryRequest.RequestedPickUpTime = request.RequestedPickUpTime;
                recoveryRequest.CanSiteAccommodate53FeetTruck = request.CanSiteAccommodate53FeetTruck;
                recoveryRequest.DoesSiteHaveADock = request.DoesSiteHaveADock;
                recoveryRequest.CanHardwareBeResold = request.CanHardwareBeResold;
                recoveryRequest.CanHardDrivesBeResold = request.CanHardDrivesBeResold;
                recoveryRequest.IsEquipmentLoose = request.IsEquipmentLoose;
                recoveryRequest.IsEquipmentPalletized = request.IsEquipmentPalletized;
                recoveryRequest.IsLiftGateRequired = request.IsLiftGateRequired;
                recoveryRequest.IsOnsiteDataDestructingRequiredWipingShredding = request.IsOnsiteDataDestructingRequiredWipingShredding;
                recoveryRequest.TypeOfMediaToBeDestroyed = request.TypeOfMediaToBeDestroyed;
                recoveryRequest.OtherInstructions = request.OtherInstructions;
                recoveryRequest.InstructionUpload = request.InstructionUpload;
                recoveryRequest.EquipmentListUpload = request.EquipmentListUpload;
                recoveryRequest.AssetsPhotoUpload1 = request.AssetsPhotoUpload1;
                recoveryRequest.AssetsPhotoUpload2 = request.AssetsPhotoUpload2;
                recoveryRequest.AssetsPhotoUpload3 = request.AssetsPhotoUpload3;
                recoveryRequest.AssetsPhotoUpload4 = request.AssetsPhotoUpload4;
                recoveryRequest.AssetsPhotoUpload5 = request.AssetsPhotoUpload5;
                recoveryRequest.HelpfulPhotoUpload1 = request.HelpfulPhotoUpload1;
                recoveryRequest.HelpfulPhotoUpload2 = request.HelpfulPhotoUpload2;
                recoveryRequest.HelpfulPhotoUpload3 = request.HelpfulPhotoUpload3;
                recoveryRequest.IsActive = request.IsActive;
                recoveryRequest.DateClosed = request.DateClosed;
                recoveryRequest.ClosureComments = request.ClosureComments;
                recoveryRequest.UpdatedBy = userId;
                recoveryRequest.DateUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating recovery request {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecoveryRequest(int id)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.RecoveryRequests
                    .Where(rr => rr.Id == id);

                if (!IsAdministrator())
                {
                    query = query.Where(rr => rr.ClientId == clientId);
                }

                var recoveryRequest = await query.FirstOrDefaultAsync();

                if (recoveryRequest == null)
                    return NotFound();

                _context.RecoveryRequests.Remove(recoveryRequest);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting recovery request {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/files")]
        public async Task<ActionResult<IEnumerable<object>>> GetUploadedFiles(int id)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.RecoveryRequests
                    .Where(rr => rr.Id == id);

                if (!IsAdministrator())
                {
                    query = query.Where(rr => rr.ClientId == clientId);
                }

                var recoveryRequest = await query.FirstOrDefaultAsync();

                if (recoveryRequest == null)
                    return NotFound();

                var uploadsPath = Path.Combine("upload", clientId, "RecoveryRequests");
                
                if (!Directory.Exists(uploadsPath))
                    return Ok(new List<object>());

                var files = Directory.GetFiles(uploadsPath)
                    .Select(filePath => 
                    {
                        var fileName = Path.GetFileName(filePath);
                        var originalFileName = fileName.Contains('_') ? fileName.Substring(fileName.IndexOf('_') + 1) : fileName;
                        var fileInfo = new FileInfo(filePath);
                        var relativePath = Path.Combine("upload", clientId, "RecoveryRequests", fileName).Replace("\\", "/");
                        
                        return new
                        {
                            fileName = originalFileName,
                            fullFileName = fileName,
                            filePath = "/" + relativePath,
                            fileSize = fileInfo.Length,
                            uploadDate = fileInfo.CreationTime,
                            documentType = "recovery_document"
                        };
                    })
                    .OrderByDescending(f => f.uploadDate)
                    .ToList();

                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving uploaded files for recovery request {Id}", id);
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

                var query = _context.RecoveryRequests
                    .Where(rr => rr.Id == id);

                if (!IsAdministrator())
                {
                    query = query.Where(rr => rr.ClientId == clientId);
                }

                var recoveryRequest = await query.FirstOrDefaultAsync();

                if (recoveryRequest == null)
                    return NotFound();

                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                var uploadsPath = Path.Combine("upload", clientId, "RecoveryRequests");
                Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                switch (documentType.ToLower())
                {
                    case "instruction":
                        recoveryRequest.InstructionUpload = fileName;
                        break;
                    case "equipmentlist":
                        recoveryRequest.EquipmentListUpload = fileName;
                        break;
                    case "assetsphoto1":
                        recoveryRequest.AssetsPhotoUpload1 = fileName;
                        break;
                    case "assetsphoto2":
                        recoveryRequest.AssetsPhotoUpload2 = fileName;
                        break;
                    case "assetsphoto3":
                        recoveryRequest.AssetsPhotoUpload3 = fileName;
                        break;
                    case "assetsphoto4":
                        recoveryRequest.AssetsPhotoUpload4 = fileName;
                        break;
                    case "assetsphoto5":
                        recoveryRequest.AssetsPhotoUpload5 = fileName;
                        break;
                    case "helpfulphoto1":
                        recoveryRequest.HelpfulPhotoUpload1 = fileName;
                        break;
                    case "helpfulphoto2":
                        recoveryRequest.HelpfulPhotoUpload2 = fileName;
                        break;
                    case "helpfulphoto3":
                        recoveryRequest.HelpfulPhotoUpload3 = fileName;
                        break;
                }

                recoveryRequest.UpdatedBy = userId;
                recoveryRequest.DateUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { id = recoveryRequest.Id, fileName = file.FileName, filePath = fileName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document for recovery request {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<int> GenerateRecoveryJobRequestId()
        {
            var lastRequest = await _context.RecoveryRequests
                .OrderByDescending(rr => rr.RecoveryJobRequestId)
                .FirstOrDefaultAsync();
            
            return (lastRequest?.RecoveryJobRequestId ?? 0) + 1;
        }
    }

    public class RecoveryRequestRequest
    {
        public bool RequestAcknowledged { get; set; }
        public string? Description { get; set; }
        public string? LocationName { get; set; }
        public string? Address { get; set; }
        public string? AddressExt { get; set; }
        public string? City { get; set; }
        public int? StateId { get; set; }
        public string? PostalCode { get; set; }
        public int? CountryId { get; set; }
        public string? PrimaryContactFirstName { get; set; }
        public string? PrimaryContactLastName { get; set; }
        public string? SecondaryContactFirstName { get; set; }
        public string? SecondaryContactLastName { get; set; }
        public string? PrimaryContactCellPhoneNumber { get; set; }
        public string? SecondaryContactCellPhoneNumber { get; set; }
        public string? PrimaryContactEmailAddress { get; set; }
        public string? SecondaryContactEmailAddress { get; set; }
        public DateTime? RequestedPickUpDate { get; set; }
        public TimeSpan? RequestedPickUpTime { get; set; }
        public bool CanSiteAccommodate53FeetTruck { get; set; }
        public bool DoesSiteHaveADock { get; set; }
        public bool CanHardwareBeResold { get; set; }
        public bool CanHardDrivesBeResold { get; set; }
        public bool IsEquipmentLoose { get; set; }
        public bool IsEquipmentPalletized { get; set; }
        public bool IsLiftGateRequired { get; set; }
        public bool IsOnsiteDataDestructingRequiredWipingShredding { get; set; }
        public string? TypeOfMediaToBeDestroyed { get; set; }
        public string? OtherInstructions { get; set; }
        public string? InstructionUpload { get; set; }
        public string? EquipmentListUpload { get; set; }
        public string? AssetsPhotoUpload1 { get; set; }
        public string? AssetsPhotoUpload2 { get; set; }
        public string? AssetsPhotoUpload3 { get; set; }
        public string? AssetsPhotoUpload4 { get; set; }
        public string? AssetsPhotoUpload5 { get; set; }
        public string? HelpfulPhotoUpload1 { get; set; }
        public string? HelpfulPhotoUpload2 { get; set; }
        public string? HelpfulPhotoUpload3 { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? DateClosed { get; set; }
        public string? ClosureComments { get; set; }
    }
}
