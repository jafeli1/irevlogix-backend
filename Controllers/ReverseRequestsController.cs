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
    public class ReverseRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReverseRequestsController> _logger;

        public ReverseRequestsController(ApplicationDbContext context, ILogger<ReverseRequestsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetClientId()
        {
            return User.FindFirst("ClientId")?.Value ?? throw new UnauthorizedAccessException("ClientId not found in token");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetReverseRequests(
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
                var query = _context.ReverseRequests
                    .Where(rr => rr.ClientId == clientId);

                if (!string.IsNullOrEmpty(primaryContactFirstName))
                    query = query.Where(rr => rr.PrimaryContactFirstName != null && rr.PrimaryContactFirstName.Contains(primaryContactFirstName));

                if (!string.IsNullOrEmpty(primaryContactLastName))
                    query = query.Where(rr => rr.PrimaryContactLastName != null && rr.PrimaryContactLastName.Contains(primaryContactLastName));

                if (!string.IsNullOrEmpty(city))
                    query = query.Where(rr => rr.City != null && rr.City.Contains(city));

                if (stateId.HasValue)
                    query = query.Where(rr => rr.StateId == stateId);

                var totalCount = await query.CountAsync();
                var reverseRequests = await query
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
                return Ok(reverseRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reverse requests");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetReverseRequest(int id)
        {
            try
            {
                var clientId = GetClientId();
                var reverseRequest = await _context.ReverseRequests
                    .Where(rr => rr.Id == id && rr.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (reverseRequest == null)
                    return NotFound();

                return Ok(reverseRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reverse request {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateReverseRequest(ReverseRequestRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var reverseRequest = new ReverseRequest
                {
                    ClientId = clientId,
                    ReverseJobRequestId = await GenerateReverseJobRequestId(),
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

                _context.ReverseRequests.Add(reverseRequest);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetReverseRequest), new { id = reverseRequest.Id }, new { id = reverseRequest.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reverse request");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReverseRequest(int id, ReverseRequestRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var reverseRequest = await _context.ReverseRequests
                    .Where(rr => rr.Id == id && rr.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (reverseRequest == null)
                    return NotFound();

                reverseRequest.RequestAcknowledged = request.RequestAcknowledged;
                reverseRequest.Description = request.Description;
                reverseRequest.LocationName = request.LocationName;
                reverseRequest.Address = request.Address;
                reverseRequest.AddressExt = request.AddressExt;
                reverseRequest.City = request.City;
                reverseRequest.StateId = request.StateId;
                reverseRequest.PostalCode = request.PostalCode;
                reverseRequest.CountryId = request.CountryId;
                reverseRequest.PrimaryContactFirstName = request.PrimaryContactFirstName;
                reverseRequest.PrimaryContactLastName = request.PrimaryContactLastName;
                reverseRequest.SecondaryContactFirstName = request.SecondaryContactFirstName;
                reverseRequest.SecondaryContactLastName = request.SecondaryContactLastName;
                reverseRequest.PrimaryContactCellPhoneNumber = request.PrimaryContactCellPhoneNumber;
                reverseRequest.SecondaryContactCellPhoneNumber = request.SecondaryContactCellPhoneNumber;
                reverseRequest.PrimaryContactEmailAddress = request.PrimaryContactEmailAddress;
                reverseRequest.SecondaryContactEmailAddress = request.SecondaryContactEmailAddress;
                reverseRequest.RequestedPickUpDate = request.RequestedPickUpDate;
                reverseRequest.RequestedPickUpTime = request.RequestedPickUpTime;
                reverseRequest.CanSiteAccommodate53FeetTruck = request.CanSiteAccommodate53FeetTruck;
                reverseRequest.DoesSiteHaveADock = request.DoesSiteHaveADock;
                reverseRequest.CanHardwareBeResold = request.CanHardwareBeResold;
                reverseRequest.CanHardDrivesBeResold = request.CanHardDrivesBeResold;
                reverseRequest.IsEquipmentLoose = request.IsEquipmentLoose;
                reverseRequest.IsEquipmentPalletized = request.IsEquipmentPalletized;
                reverseRequest.IsLiftGateRequired = request.IsLiftGateRequired;
                reverseRequest.IsOnsiteDataDestructingRequiredWipingShredding = request.IsOnsiteDataDestructingRequiredWipingShredding;
                reverseRequest.TypeOfMediaToBeDestroyed = request.TypeOfMediaToBeDestroyed;
                reverseRequest.OtherInstructions = request.OtherInstructions;
                reverseRequest.InstructionUpload = request.InstructionUpload;
                reverseRequest.EquipmentListUpload = request.EquipmentListUpload;
                reverseRequest.AssetsPhotoUpload1 = request.AssetsPhotoUpload1;
                reverseRequest.AssetsPhotoUpload2 = request.AssetsPhotoUpload2;
                reverseRequest.AssetsPhotoUpload3 = request.AssetsPhotoUpload3;
                reverseRequest.AssetsPhotoUpload4 = request.AssetsPhotoUpload4;
                reverseRequest.AssetsPhotoUpload5 = request.AssetsPhotoUpload5;
                reverseRequest.HelpfulPhotoUpload1 = request.HelpfulPhotoUpload1;
                reverseRequest.HelpfulPhotoUpload2 = request.HelpfulPhotoUpload2;
                reverseRequest.HelpfulPhotoUpload3 = request.HelpfulPhotoUpload3;
                reverseRequest.IsActive = request.IsActive;
                reverseRequest.DateClosed = request.DateClosed;
                reverseRequest.ClosureComments = request.ClosureComments;
                reverseRequest.UpdatedBy = userId;
                reverseRequest.DateUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating reverse request {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReverseRequest(int id)
        {
            try
            {
                var clientId = GetClientId();
                var reverseRequest = await _context.ReverseRequests
                    .Where(rr => rr.Id == id && rr.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (reverseRequest == null)
                    return NotFound();

                _context.ReverseRequests.Remove(reverseRequest);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting reverse request {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/files")]
        public async Task<ActionResult<IEnumerable<object>>> GetUploadedFiles(int id)
        {
            try
            {
                var clientId = GetClientId();
                var reverseRequest = await _context.ReverseRequests
                    .Where(rr => rr.Id == id && rr.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (reverseRequest == null)
                    return NotFound();

                var uploadsPath = Path.Combine("upload", clientId, "ReverseRequests");
                
                if (!Directory.Exists(uploadsPath))
                    return Ok(new List<object>());

                var files = Directory.GetFiles(uploadsPath)
                    .Select(filePath => 
                    {
                        var fileName = Path.GetFileName(filePath);
                        var originalFileName = fileName.Contains('_') ? fileName.Substring(fileName.IndexOf('_') + 1) : fileName;
                        var fileInfo = new FileInfo(filePath);
                        var relativePath = Path.Combine("upload", clientId, "ReverseRequests", fileName).Replace("\\", "/");
                        
                        return new
                        {
                            fileName = originalFileName,
                            fullFileName = fileName,
                            filePath = "/" + relativePath,
                            fileSize = fileInfo.Length,
                            uploadDate = fileInfo.CreationTime,
                            documentType = "reverse_document"
                        };
                    })
                    .OrderByDescending(f => f.uploadDate)
                    .ToList();

                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving uploaded files for reverse request {Id}", id);
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

                var reverseRequest = await _context.ReverseRequests
                    .Where(rr => rr.Id == id && rr.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (reverseRequest == null)
                    return NotFound();

                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                var uploadsPath = Path.Combine("upload", clientId, "ReverseRequests");
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
                        reverseRequest.InstructionUpload = fileName;
                        break;
                    case "equipmentlist":
                        reverseRequest.EquipmentListUpload = fileName;
                        break;
                    case "assetsphoto1":
                        reverseRequest.AssetsPhotoUpload1 = fileName;
                        break;
                    case "assetsphoto2":
                        reverseRequest.AssetsPhotoUpload2 = fileName;
                        break;
                    case "assetsphoto3":
                        reverseRequest.AssetsPhotoUpload3 = fileName;
                        break;
                    case "assetsphoto4":
                        reverseRequest.AssetsPhotoUpload4 = fileName;
                        break;
                    case "assetsphoto5":
                        reverseRequest.AssetsPhotoUpload5 = fileName;
                        break;
                    case "helpfulphoto1":
                        reverseRequest.HelpfulPhotoUpload1 = fileName;
                        break;
                    case "helpfulphoto2":
                        reverseRequest.HelpfulPhotoUpload2 = fileName;
                        break;
                    case "helpfulphoto3":
                        reverseRequest.HelpfulPhotoUpload3 = fileName;
                        break;
                }

                reverseRequest.UpdatedBy = userId;
                reverseRequest.DateUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { id = reverseRequest.Id, fileName = file.FileName, filePath = fileName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document for reverse request {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("dropdown")]
        public async Task<ActionResult<IEnumerable<object>>> GetReverseRequestsForDropdown()
        {
            try
            {
                var clientId = GetClientId();
                var reverseRequests = await _context.ReverseRequests
                    .Where(rr => rr.ClientId == clientId && rr.IsActive)
                    .Select(rr => new
                    {
                        rr.Id,
                        rr.LocationName
                    })
                    .OrderBy(rr => rr.LocationName)
                    .ToListAsync();

                return Ok(reverseRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reverse requests for dropdown");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<int> GenerateReverseJobRequestId()
        {
            var lastRequest = await _context.ReverseRequests
                .OrderByDescending(rr => rr.ReverseJobRequestId)
                .FirstOrDefaultAsync();
            
            return (lastRequest?.ReverseJobRequestId ?? 0) + 1;
        }
    }

    public class ReverseRequestRequest
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

        [HttpGet("{id}/files")]
        public async Task<ActionResult<IEnumerable<object>>> GetUploadedFiles(int id)
        {
            try
            {
                var clientId = GetClientId();
                var request = await _context.ReverseRequests
                    .Where(rr => rr.Id == id && rr.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (request == null)
                    return NotFound();

                var uploadsPath = Path.Combine("upload", clientId, "ReverseRequests");
                
                if (!Directory.Exists(uploadsPath))
                    return Ok(new List<object>());

                var files = Directory.GetFiles(uploadsPath)
                    .Select(filePath => 
                    {
                        var fileName = Path.GetFileName(filePath);
                        var originalFileName = fileName.Contains('_') ? fileName.Substring(fileName.IndexOf('_') + 1) : fileName;
                        var fileInfo = new FileInfo(filePath);
                        var relativePath = Path.Combine("upload", clientId, "ReverseRequests", fileName).Replace("\\", "/");
                        
                        return new
                        {
                            fileName = originalFileName,
                            fullFileName = fileName,
                            filePath = "/" + relativePath,
                            fileSize = fileInfo.Length,
                            uploadDate = fileInfo.CreationTime,
                            documentType = "reverse_request_document"
                        };
                    })
                    .OrderByDescending(f => f.uploadDate)
                    .ToList();

                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving uploaded files for reverse request {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
