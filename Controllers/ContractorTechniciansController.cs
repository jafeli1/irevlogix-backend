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
    public class ContractorTechniciansController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ContractorTechniciansController> _logger;

        public ContractorTechniciansController(ApplicationDbContext context, ILogger<ContractorTechniciansController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetClientId()
        {
            return User.FindFirst("ClientId")?.Value ?? throw new UnauthorizedAccessException("ClientId not found in token");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetContractorTechnicians(
            [FromQuery] string? firstName = null,
            [FromQuery] string? lastName = null,
            [FromQuery] string? city = null,
            [FromQuery] int? stateId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clientId = GetClientId();
                var query = _context.ContractorTechnicians
                    .Include(ct => ct.User)
                    .Where(ct => ct.ClientId == clientId);

                if (!string.IsNullOrEmpty(firstName))
                    query = query.Where(ct => ct.FirstName.Contains(firstName));

                if (!string.IsNullOrEmpty(lastName))
                    query = query.Where(ct => ct.LastName.Contains(lastName));

                if (!string.IsNullOrEmpty(city))
                    query = query.Where(ct => ct.City != null && ct.City.Contains(city));

                if (stateId.HasValue)
                    query = query.Where(ct => ct.StateId == stateId);

                var totalCount = await query.CountAsync();
                var contractors = await query
                    .OrderBy(ct => ct.LastName)
                    .ThenBy(ct => ct.FirstName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(ct => new
                    {
                        ct.Id,
                        ct.FirstName,
                        ct.LastName,
                        ct.City,
                        ct.StateId,
                        ct.Phone,
                        ct.Email,
                        ct.Approved,
                        ct.Preferred,
                        ct.DateCreated,
                        ct.DateUpdated
                    })
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());
                return Ok(contractors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contractor technicians");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetContractorTechnician(int id)
        {
            try
            {
                var clientId = GetClientId();
                var contractor = await _context.ContractorTechnicians
                    .Include(ct => ct.User)
                    .Where(ct => ct.Id == id && ct.ClientId == clientId)
                    .Select(ct => new
                    {
                        ct.Id,
                        ct.UserId,
                        ct.ClientId,
                        ct.Approved,
                        ct.Preferred,
                        ct.TechnicianSource,
                        ct.Comments,
                        ct.FirstName,
                        ct.LastName,
                        ct.City,
                        ct.StateId,
                        ct.ZipCode,
                        ct.Phone,
                        ct.Email,
                        ct.ShippingAddress,
                        ct.OnboardDate,
                        ct.ExpirationDate,
                        ct.BackgroundCheckOnboarding,
                        ct.DrugTestOnboarding,
                        ct.ThirdPartyAgreementOnboarding,
                        ct.BackgroundCheckDate,
                        ct.DrugTestDate,
                        ct.ThirdPartyServiceProviderAgreementVersion,
                        ct.TrainingCompletionDate,
                        ct.BackgroundCheckFormUpload,
                        ct.DrugTestUpload,
                        ct.ThirdPartyServiceProviderAgreementUpload,
                        ct.MiscUpload,
                        ct.UpdateSummary,
                        ct.DateCreated,
                        ct.DateUpdated,
                        ct.CreatedBy,
                        ct.UpdatedBy
                    })
                    .FirstOrDefaultAsync();

                if (contractor == null)
                    return NotFound();

                return Ok(contractor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contractor technician {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> CreateContractorTechnician(ContractorTechnicianRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var contractor = new ContractorTechnician
                {
                    UserId = request.UserId,
                    ClientId = clientId,
                    Approved = request.Approved,
                    Preferred = request.Preferred,
                    TechnicianSource = request.TechnicianSource,
                    Comments = request.Comments,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    City = request.City,
                    StateId = request.StateId,
                    ZipCode = request.ZipCode,
                    Phone = request.Phone,
                    Email = request.Email,
                    ShippingAddress = request.ShippingAddress,
                    OnboardDate = request.OnboardDate,
                    ExpirationDate = request.ExpirationDate,
                    BackgroundCheckOnboarding = request.BackgroundCheckOnboarding,
                    DrugTestOnboarding = request.DrugTestOnboarding,
                    ThirdPartyAgreementOnboarding = request.ThirdPartyAgreementOnboarding,
                    BackgroundCheckDate = request.BackgroundCheckDate,
                    DrugTestDate = request.DrugTestDate,
                    ThirdPartyServiceProviderAgreementVersion = request.ThirdPartyServiceProviderAgreementVersion,
                    TrainingCompletionDate = request.TrainingCompletionDate,
                    BackgroundCheckFormUpload = request.BackgroundCheckFormUpload,
                    DrugTestUpload = request.DrugTestUpload,
                    ThirdPartyServiceProviderAgreementUpload = request.ThirdPartyServiceProviderAgreementUpload,
                    MiscUpload = request.MiscUpload,
                    UpdateSummary = request.UpdateSummary,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.ContractorTechnicians.Add(contractor);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetContractorTechnician), new { id = contractor.Id }, new { id = contractor.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contractor technician");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContractorTechnician(int id, ContractorTechnicianRequest request)
        {
            try
            {
                var clientId = GetClientId();
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var contractor = await _context.ContractorTechnicians
                    .Where(ct => ct.Id == id && ct.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (contractor == null)
                    return NotFound();

                contractor.UserId = request.UserId;
                contractor.Approved = request.Approved;
                contractor.Preferred = request.Preferred;
                contractor.TechnicianSource = request.TechnicianSource;
                contractor.Comments = request.Comments;
                contractor.FirstName = request.FirstName;
                contractor.LastName = request.LastName;
                contractor.City = request.City;
                contractor.StateId = request.StateId;
                contractor.ZipCode = request.ZipCode;
                contractor.Phone = request.Phone;
                contractor.Email = request.Email;
                contractor.ShippingAddress = request.ShippingAddress;
                contractor.OnboardDate = request.OnboardDate;
                contractor.ExpirationDate = request.ExpirationDate;
                contractor.BackgroundCheckOnboarding = request.BackgroundCheckOnboarding;
                contractor.DrugTestOnboarding = request.DrugTestOnboarding;
                contractor.ThirdPartyAgreementOnboarding = request.ThirdPartyAgreementOnboarding;
                contractor.BackgroundCheckDate = request.BackgroundCheckDate;
                contractor.DrugTestDate = request.DrugTestDate;
                contractor.ThirdPartyServiceProviderAgreementVersion = request.ThirdPartyServiceProviderAgreementVersion;
                contractor.TrainingCompletionDate = request.TrainingCompletionDate;
                contractor.BackgroundCheckFormUpload = request.BackgroundCheckFormUpload;
                contractor.DrugTestUpload = request.DrugTestUpload;
                contractor.ThirdPartyServiceProviderAgreementUpload = request.ThirdPartyServiceProviderAgreementUpload;
                contractor.MiscUpload = request.MiscUpload;
                contractor.UpdateSummary = request.UpdateSummary;
                contractor.UpdatedBy = userId;
                contractor.DateUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contractor technician {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContractorTechnician(int id)
        {
            try
            {
                var clientId = GetClientId();
                var contractor = await _context.ContractorTechnicians
                    .Where(ct => ct.Id == id && ct.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (contractor == null)
                    return NotFound();

                _context.ContractorTechnicians.Remove(contractor);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contractor technician {Id}", id);
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

                var contractor = await _context.ContractorTechnicians
                    .Where(ct => ct.Id == id && ct.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (contractor == null)
                    return NotFound();

                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                var uploadsPath = Path.Combine("upload", clientId, "ContractorTechnicians");
                Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var document = new ContractorTechnicianDocument
                {
                    ContractorTechnicianId = id,
                    FileName = file.FileName,
                    FilePath = filePath,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    Description = description,
                    DocumentType = documentType,
                    ClientId = clientId,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };

                _context.ContractorTechnicianDocuments.Add(document);
                await _context.SaveChangesAsync();

                return Ok(new { id = document.Id, fileName = file.FileName, filePath = fileName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document for contractor technician {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/files")]
        public async Task<ActionResult<IEnumerable<object>>> GetUploadedFiles(int id)
        {
            try
            {
                var clientId = GetClientId();
                var contractor = await _context.ContractorTechnicians
                    .Where(ct => ct.Id == id && ct.ClientId == clientId)
                    .FirstOrDefaultAsync();

                if (contractor == null)
                    return NotFound();

                var uploadsPath = Path.Combine("upload", clientId, "ContractorTechnicians");
                
                if (!Directory.Exists(uploadsPath))
                    return Ok(new List<object>());

                var files = Directory.GetFiles(uploadsPath)
                    .Select(filePath => 
                    {
                        var fileName = Path.GetFileName(filePath);
                        var originalFileName = fileName.Contains('_') ? fileName.Substring(fileName.IndexOf('_') + 1) : fileName;
                        var fileInfo = new FileInfo(filePath);
                        var relativePath = Path.Combine("upload", clientId, "ContractorTechnicians", fileName).Replace("\\", "/");
                        
                        return new
                        {
                            fileName = originalFileName,
                            fullFileName = fileName,
                            filePath = "/" + relativePath,
                            fileSize = fileInfo.Length,
                            uploadDate = fileInfo.CreationTime,
                            documentType = "contractor_document"
                        };
                    })
                    .OrderByDescending(f => f.uploadDate)
                    .ToList();

                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving uploaded files for contractor technician {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class ContractorTechnicianRequest
    {
        public int UserId { get; set; }
        public bool Approved { get; set; }
        public bool Preferred { get; set; }
        public string? TechnicianSource { get; set; }
        public string? Comments { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? City { get; set; }
        public int? StateId { get; set; }
        public string? ZipCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? ShippingAddress { get; set; }
        public DateTime? OnboardDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool BackgroundCheckOnboarding { get; set; }
        public bool DrugTestOnboarding { get; set; }
        public bool ThirdPartyAgreementOnboarding { get; set; }
        public DateTime? BackgroundCheckDate { get; set; }
        public DateTime? DrugTestDate { get; set; }
        public DateTime? ThirdPartyServiceProviderAgreementVersion { get; set; }
        public DateTime? TrainingCompletionDate { get; set; }
        public string? BackgroundCheckFormUpload { get; set; }
        public string? DrugTestUpload { get; set; }
        public string? ThirdPartyServiceProviderAgreementUpload { get; set; }
        public string? MiscUpload { get; set; }
        public string? UpdateSummary { get; set; }
    }

    [HttpGet("{id}/files")]
    public async Task<ActionResult<IEnumerable<object>>> GetUploadedFiles(int id)
    {
        try
        {
            var clientId = GetClientId();
            var contractor = await _context.ContractorTechnicians
                .Where(ct => ct.Id == id && ct.ClientId == clientId)
                .FirstOrDefaultAsync();

            if (contractor == null)
                return NotFound();

            var uploadsPath = Path.Combine("upload", clientId, "ContractorTechnicians");
            
            if (!Directory.Exists(uploadsPath))
                return Ok(new List<object>());

            var files = Directory.GetFiles(uploadsPath)
                .Select(filePath => 
                {
                    var fileName = Path.GetFileName(filePath);
                    var originalFileName = fileName.Contains('_') ? fileName.Substring(fileName.IndexOf('_') + 1) : fileName;
                    var fileInfo = new FileInfo(filePath);
                    var relativePath = Path.Combine("upload", clientId, "ContractorTechnicians", fileName).Replace("\\", "/");
                    
                    return new
                    {
                        fileName = originalFileName,
                        fullFileName = fileName,
                        filePath = "/" + relativePath,
                        fileSize = fileInfo.Length,
                        uploadDate = fileInfo.CreationTime,
                        documentType = "contractor_document"
                    };
                })
                .OrderByDescending(f => f.uploadDate)
                .ToList();

            return Ok(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving uploaded files for contractor technician {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
