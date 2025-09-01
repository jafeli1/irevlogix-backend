using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using irevlogix_backend.Models;
using System.Security.Claims;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProcessedMaterialTestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProcessedMaterialTestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] int? processedMaterialId = null)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var query = _context.ProcessedMaterialTests
                .Include(x => x.ProcessedMaterial)
                .ThenInclude(x => x.MaterialType)
                .Where(x => x.ClientId == clientId);

            if (processedMaterialId.HasValue)
            {
                query = query.Where(x => x.ProcessedMaterialId == processedMaterialId.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.DateCreated)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.ProcessedMaterialId,
                    ProcessedMaterial = x.ProcessedMaterial == null ? null : new
                    {
                        x.ProcessedMaterial.Id,
                        x.ProcessedMaterial.Description,
                        MaterialType = x.ProcessedMaterial.MaterialType == null ? null : new
                        {
                            x.ProcessedMaterial.MaterialType.Id,
                            x.ProcessedMaterial.MaterialType.Name
                        }
                    },
                    x.TestDate,
                    x.Lab,
                    x.Parameters,
                    x.Results,
                    x.ComplianceStatus,
                    x.ReportDocumentUrl
                })
                .ToListAsync();

            return Ok(new { items, totalCount, page, pageSize });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var item = await _context.ProcessedMaterialTests
                .Include(x => x.ProcessedMaterial)
                .ThenInclude(x => x.MaterialType)
                .FirstOrDefaultAsync(x => x.Id == id && x.ClientId == clientId);

            if (item == null) return NotFound();

            return Ok(new
            {
                item.Id,
                item.ProcessedMaterialId,
                ProcessedMaterial = item.ProcessedMaterial == null ? null : new
                {
                    item.ProcessedMaterial.Id,
                    item.ProcessedMaterial.Description,
                    MaterialType = item.ProcessedMaterial.MaterialType == null ? null : new
                    {
                        item.ProcessedMaterial.MaterialType.Id,
                        item.ProcessedMaterial.MaterialType.Name
                    }
                },
                item.TestDate,
                item.Lab,
                item.Parameters,
                item.Results,
                item.ComplianceStatus,
                item.ReportDocumentUrl
            });
        }

        public class CreateProcessedMaterialTestsDto
        {
            public int ProcessedMaterialId { get; set; }
            public DateTime? TestDate { get; set; }
            public string? Lab { get; set; }
            public string? Parameters { get; set; }
            public string? Results { get; set; }
            public string? ComplianceStatus { get; set; }
            public string? ReportDocumentUrl { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProcessedMaterialTestsDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(userId)) return Unauthorized();

            var entity = new ProcessedMaterialTests
            {
                ProcessedMaterialId = dto.ProcessedMaterialId,
                TestDate = dto.TestDate,
                Lab = dto.Lab,
                Parameters = dto.Parameters,
                Results = dto.Results,
                ComplianceStatus = dto.ComplianceStatus,
                ReportDocumentUrl = dto.ReportDocumentUrl,
                ClientId = clientId,
                CreatedBy = int.Parse(userId),
                UpdatedBy = int.Parse(userId),
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            _context.ProcessedMaterialTests.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        public class UpdateProcessedMaterialTestsDto
        {
            public DateTime? TestDate { get; set; }
            public string? Lab { get; set; }
            public string? Parameters { get; set; }
            public string? Results { get; set; }
            public string? ComplianceStatus { get; set; }
            public string? ReportDocumentUrl { get; set; }
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch([FromRoute] int id, [FromBody] UpdateProcessedMaterialTestsDto dto)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var entity = await _context.ProcessedMaterialTests.FirstOrDefaultAsync(x => x.Id == id && x.ClientId == clientId);
            if (entity == null) return NotFound();

            if (dto.TestDate.HasValue) entity.TestDate = dto.TestDate;
            if (!string.IsNullOrWhiteSpace(dto.Lab)) entity.Lab = dto.Lab;
            if (!string.IsNullOrWhiteSpace(dto.Parameters)) entity.Parameters = dto.Parameters;
            if (!string.IsNullOrWhiteSpace(dto.Results)) entity.Results = dto.Results;
            if (!string.IsNullOrWhiteSpace(dto.ComplianceStatus)) entity.ComplianceStatus = dto.ComplianceStatus;
            if (!string.IsNullOrWhiteSpace(dto.ReportDocumentUrl)) entity.ReportDocumentUrl = dto.ReportDocumentUrl;
            entity.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var entity = await _context.ProcessedMaterialTests.FirstOrDefaultAsync(x => x.Id == id && x.ClientId == clientId);
            if (entity == null) return NotFound();

            _context.ProcessedMaterialTests.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("files")]
        public async Task<ActionResult<IEnumerable<object>>> GetUploadedFiles()
        {
            try
            {
                var clientId = User.FindFirst("ClientId")?.Value;
                if (string.IsNullOrEmpty(clientId)) return Unauthorized();

                var uploadsPath = Path.Combine("upload", clientId, "ProcessedMaterialTests");
                
                if (!Directory.Exists(uploadsPath))
                    return Ok(new List<object>());

                var files = Directory.GetFiles(uploadsPath)
                    .Select(filePath => 
                    {
                        var fileName = Path.GetFileName(filePath);
                        var originalFileName = fileName.Contains('_') ? fileName.Substring(fileName.IndexOf('_') + 1) : fileName;
                        var fileInfo = new FileInfo(filePath);
                        var relativePath = Path.Combine("upload", clientId, "ProcessedMaterialTests", fileName).Replace("\\", "/");
                        
                        return new
                        {
                            fileName = originalFileName,
                            fullFileName = fileName,
                            filePath = "/" + relativePath,
                            fileSize = fileInfo.Length,
                            uploadDate = fileInfo.CreationTime,
                            documentType = "test_document"
                        };
                    })
                    .OrderByDescending(f => f.uploadDate)
                    .ToList();

                return Ok(files);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Invalid file type. Allowed types: PDF, DOC, DOCX, JPG, JPEG, PNG");

            var uploadsPath = Path.Combine("upload", clientId, "ProcessedMaterialTests");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("upload", clientId, "ProcessedMaterialTests", fileName).Replace("\\", "/");
            return Ok(new { filePath = relativePath });
        }
    }

}
